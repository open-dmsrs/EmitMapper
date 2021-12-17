using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using EmitMapper.AST;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using EmitMapper.AST.Nodes;
using EmitMapper.Conversion;
using EmitMapper.Mappers;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;
using EmitMapper.Utils;

namespace EmitMapper.EmitBuilders;

internal class MappingOperationsProcessor
{
    public CompilationContext CompilationContext;

    public IEnumerable<IMappingOperation> Operations = new List<IMappingOperation>();

    public IMappingConfigurator MappingConfigurator;

    public IRootMappingOperation RootOperation;

    public List<object> StoredObjects = new();

    public LocalBuilder LocException;

    public LocalBuilder LocFrom;

    public LocalBuilder LocState;

    public LocalBuilder LocTo;

    public ObjectMapperManager ObjectsMapperManager;

    public StaticConvertersManager StaticConvertersManager;

    public MappingOperationsProcessor()
    {
    }

    public MappingOperationsProcessor(MappingOperationsProcessor prototype)
    {
        LocFrom = prototype.LocFrom;
        LocTo = prototype.LocTo;
        LocState = prototype.LocState;
        LocException = prototype.LocException;
        CompilationContext = prototype.CompilationContext;
        Operations = prototype.Operations;
        StoredObjects = prototype.StoredObjects;
        MappingConfigurator = prototype.MappingConfigurator;
        ObjectsMapperManager = prototype.ObjectsMapperManager;
        RootOperation = prototype.RootOperation;
        StaticConvertersManager = prototype.StaticConvertersManager;
    }

    private static IAstRef GetStoredObject(int objectIndex, Type castType)
    {
        var result = (IAstRef)AstBuildHelper.ReadArrayItemRV(
            (IAstRef)AstBuildHelper.ReadFieldRA(
                new AstReadThis
                {
                    ThisType = typeof(ObjectsMapperBaseImpl)
                },
                typeof(ObjectsMapperBaseImpl).GetField(
                    nameof(ObjectsMapperBaseImpl.StroredObjects),
                    BindingFlags.Instance | BindingFlags.Public)),
            objectIndex);
        if (castType != null) result = new AstCastclassRef(result, castType);
        return result;
    }

    public IAstNode ProcessOperations()
    {
        var result = new AstComplexNode();
        foreach (var operation in Operations)
        {
            IAstNode completeOperation = null;
            var operationId = AddObjectToStore(operation);

            if (operation is OperationsBlock block)
                completeOperation = new MappingOperationsProcessor(this)
                {
                    Operations = block.Operations
                }.ProcessOperations();
            else if (operation is ReadWriteComplex complex)
                completeOperation = Process_ReadWriteComplex(complex, operationId);
            else if (operation is DestSrcReadOperation readOperation)
                completeOperation = ProcessDestSrcReadOperation(readOperation, operationId);
            else if (operation is SrcReadOperation srcReadOperation)
                completeOperation = ProcessSrcReadOperation(srcReadOperation, operationId);
            else if (operation is DestWriteOperation writeOperation)
                completeOperation = ProcessDestWriteOperation(writeOperation, operationId);
            else if (operation is ReadWriteSimple simple)
                completeOperation = ProcessReadWriteSimple(simple, operationId);

            if (completeOperation == null) continue;

            if (LocException != null)
            {
                var tryCatch = CreateExceptionHandlingBlock(operationId, completeOperation);
                result.Nodes.Add(tryCatch);
            }
            else
            {
                result.Nodes.Add(completeOperation);
            }
        }

        return result;
    }

    private IAstNode ProcessReadWriteSimple(ReadWriteSimple readWriteSimple, int operationId)
    {
        var sourceValue = ReadSrcMappingValue(readWriteSimple, operationId);

        IAstRefOrValue convertedValue;

        if (readWriteSimple.NullSubstitutor != null && (ReflectionUtils.IsNullable(readWriteSimple.Source.MemberType) ||
                                                        !readWriteSimple.Source.MemberType.IsValueType))
            convertedValue = new AstIfTernar(
                ReflectionUtils.IsNullable(readWriteSimple.Source.MemberType)
                    ? new AstExprNot(
                        AstBuildHelper.ReadPropertyRV(
                            new AstValueToAddr((IAstValue)sourceValue),
                            readWriteSimple.Source.MemberType.GetProperty("HasValue")))
                    : new AstExprIsNull(sourceValue),
                GetNullValue(readWriteSimple.NullSubstitutor), // source is null
                AstBuildHelper.CastClass(
                    ConvertMappingValue(readWriteSimple, operationId, sourceValue),
                    readWriteSimple.Destination.MemberType));
        else
            convertedValue = ConvertMappingValue(readWriteSimple, operationId, sourceValue);

        var result = WriteMappingValue(readWriteSimple, operationId, convertedValue);

        if (readWriteSimple.SourceFilter != null) result = Process_SourceFilter(readWriteSimple, operationId, result);

        if (readWriteSimple.DestinationFilter != null)
            result = Process_DestinationFilter(readWriteSimple, operationId, result);
        return result;
    }

    private IAstNode ProcessDestWriteOperation(DestWriteOperation destWriteOperation, int operationId)
    {
        var locValueToWrite = CompilationContext.ILGenerator.DeclareLocal(destWriteOperation.Getter.Method.ReturnType);

        var cmdValue = new AstWriteLocal(
            locValueToWrite,
            AstBuildHelper.CallMethod(
                destWriteOperation.Getter.GetType().GetMethod("Invoke"),
                new AstCastclassRef(
                    (IAstRef)AstBuildHelper.ReadMemberRV(
                        GetStoredObject(operationId, typeof(DestWriteOperation)),
                        typeof(DestWriteOperation).GetProperty(nameof(DestWriteOperation.Getter))),
                    destWriteOperation.Getter.GetType()),
                new List<IAstStackItem>
                {
                    AstBuildHelper.ReadLocalRV(LocFrom), AstBuildHelper.ReadLocalRV(LocState)
                }));
        //todo: need to add unit test for this method
        return new AstComplexNode
        {
            Nodes = new List<IAstNode>
            {
                cmdValue,
                new AstIf
                {
                    Condition = new AstExprEquals(
                        (IAstValue)AstBuildHelper.ReadMembersChain(
                            AstBuildHelper.ReadLocalRA(locValueToWrite),
                            new[]
                            {
                                (MemberInfo)locValueToWrite.LocalType.GetField(nameof(ValueToWrite<object>.Action))
                            }),
                        new AstConstantInt32
                        {
                            Value = 0
                        }),
                    TrueBranch = new AstComplexNode
                    {
                        Nodes = new List<IAstNode>
                        {
                            AstBuildHelper.WriteMembersChain(
                                destWriteOperation.Destination.MembersChain,
                                AstBuildHelper.ReadLocalRA(LocTo),
                                AstBuildHelper.ReadMembersChain(
                                    AstBuildHelper.ReadLocalRA(locValueToWrite),
                                    new[]
                                    {
                                        (MemberInfo)locValueToWrite.LocalType.GetField(
                                            nameof(ValueToWrite<object>.Value)) // changed to Value from value
                                    }))
                        }
                    }
                }
            }
        };
    }

    private IAstNode ProcessSrcReadOperation(SrcReadOperation srcReadOperation, int operationId)
    {
        var value = AstBuildHelper.ReadMembersChain(
            AstBuildHelper.ReadLocalRA(LocFrom),
            srcReadOperation.Source.MembersChain);

        return WriteMappingValue(srcReadOperation, operationId, value);
    }

    private IAstNode Process_ReadWriteComplex(ReadWriteComplex op, int operationId)
    {
        IAstNode result;
        if (op.Converter != null)
            result = Process_ReadWriteComplex_ByConverter(op, operationId);
        else
            result = Process_ReadWriteComplex_Copying(op);

        if (op.SourceFilter != null) result = Process_SourceFilter(op, operationId, result);

        if (op.DestinationFilter != null) result = Process_DestinationFilter(op, operationId, result);

        return result;
    }

    private IAstNode Process_DestinationFilter(IReadWriteOperation op, int operationId, IAstNode result)
    {
        return Process_ValuesFilter(
            op,
            operationId,
            result,
            AstBuildHelper.ReadMembersChain(AstBuildHelper.ReadLocalRA(LocTo), op.Destination.MembersChain),
            nameof(IReadWriteOperation.DestinationFilter),
            op.DestinationFilter);
    }

    private IAstNode Process_SourceFilter(IReadWriteOperation op, int operationId, IAstNode result)
    {
        return Process_ValuesFilter(
            op,
            operationId,
            result,
            AstBuildHelper.ReadMembersChain(AstBuildHelper.ReadLocalRA(LocFrom), op.Source.MembersChain),
            nameof(IReadWriteOperation.SourceFilter),
            op.SourceFilter);
    }

    private IAstNode Process_ValuesFilter(
        IReadWriteOperation op,
        int operationId,
        IAstNode result,
        IAstRefOrValue value,
        string fieldName,
        Delegate filterDelegate)
    {
        result = new AstComplexNode
        {
            Nodes = new List<IAstNode>
            {
                new AstIf
                {
                    Condition = (IAstValue)AstBuildHelper.CallMethod(
                        filterDelegate.GetType().GetMethod("Invoke"),
                        new AstCastclassRef(
                            (IAstRef)AstBuildHelper.ReadMemberRV(
                                GetStoredObject(operationId, typeof(IReadWriteOperation)),
                                typeof(IReadWriteOperation).GetProperty(fieldName)),
                            filterDelegate.GetType()),
                        new List<IAstStackItem>
                        {
                            value, AstBuildHelper.ReadLocalRV(LocState)
                        }),
                    TrueBranch = new AstComplexNode
                    {
                        Nodes = new List<IAstNode>
                        {
                            result
                        }
                    }
                }
            }
        };
        return result;
    }

    private IAstNode Process_ReadWriteComplex_Copying(ReadWriteComplex op)
    {
        var result = new AstComplexNode();
        var tempSrc = CompilationContext.ILGenerator.DeclareLocal(op.Source.MemberType);
        var tempDst = CompilationContext.ILGenerator.DeclareLocal(op.Destination.MemberType);
        var origTempSrc = tempSrc;
        var origTempDst = tempDst;

        result.Nodes.Add(
            new AstWriteLocal(
                tempSrc,
                AstBuildHelper.ReadMembersChain(AstBuildHelper.ReadLocalRA(LocFrom), op.Source.MembersChain)));
        result.Nodes.Add(
            new AstWriteLocal(
                tempDst,
                AstBuildHelper.ReadMembersChain(AstBuildHelper.ReadLocalRA(LocTo), op.Destination.MembersChain)));

        var writeNullToDest = new List<IAstNode>
        {
            AstBuildHelper.WriteMembersChain(
                op.Destination.MembersChain,
                AstBuildHelper.ReadLocalRA(LocTo),
                GetNullValue(op.NullSubstitutor))
        };

        // Target construction

        var initDest = new List<IAstNode>();
        var custCtr = op.TargetConstructor;
        if (custCtr != null)
        {
            var custCtrIdx = AddObjectToStore(custCtr);
            initDest.Add(
                new AstWriteLocal(
                    tempDst,
                    AstBuildHelper.CallMethod(
                        custCtr.GetType().GetMethod("Invoke"),
                        GetStoredObject(custCtrIdx, custCtr.GetType()),
                        null)));
        }
        else
        {
            initDest.Add(new AstWriteLocal(tempDst, new AstNewObject(op.Destination.MemberType, null)));
        }

        var copying = new List<IAstNode>();

        // if destination is nullable, create a temp target variable with underlying destination type
        if (ReflectionUtils.IsNullable(op.Source.MemberType))
        {
            tempSrc = CompilationContext.ILGenerator.DeclareLocal(Nullable.GetUnderlyingType(op.Source.MemberType));
            copying.Add(
                new AstWriteLocal(
                    tempSrc,
                    AstBuildHelper.ReadPropertyRV(
                        AstBuildHelper.ReadLocalRA(origTempSrc),
                        op.Source.MemberType.GetProperty("Value"))));
        }

        // If destination is null, initialize it.
        if (ReflectionUtils.IsNullable(op.Destination.MemberType) || !op.Destination.MemberType.IsValueType)
        {
            copying.Add(
                new AstIf
                {
                    Condition = ReflectionUtils.IsNullable(op.Destination.MemberType)
                        ? new AstExprNot(
                            (IAstValue)AstBuildHelper.ReadPropertyRV(
                                AstBuildHelper.ReadLocalRA(origTempDst),
                                op.Destination.MemberType.GetProperty("HasValue")))
                        : new AstExprIsNull(AstBuildHelper.ReadLocalRV(origTempDst)),
                    TrueBranch = new AstComplexNode
                    {
                        Nodes = initDest
                    }
                });
            if (ReflectionUtils.IsNullable(op.Destination.MemberType))
            {
                tempDst = CompilationContext.ILGenerator.DeclareLocal(
                    Nullable.GetUnderlyingType(op.Destination.MemberType));
                copying.Add(
                    new AstWriteLocal(
                        tempDst,
                        AstBuildHelper.ReadPropertyRV(
                            AstBuildHelper.ReadLocalRA(origTempDst),
                            op.Destination.MemberType.GetProperty("Value"))));
            }
        }

        // Suboperations
        copying.Add(
            new AstComplexNode
            {
                Nodes = new List<IAstNode>
                {
                    new MappingOperationsProcessor(this)
                    {
                        Operations = op.Operations,
                        LocTo = tempDst,
                        LocFrom = tempSrc,
                        RootOperation =
                            MappingConfigurator.GetRootMappingOperation(op.Source.MemberType, op.Destination.MemberType)
                    }.ProcessOperations()
                }
            });

        IAstRefOrValue processedValue;
        if (ReflectionUtils.IsNullable(op.Destination.MemberType))
            processedValue = new AstNewObject(
                op.Destination.MemberType,
                new[]
                {
                    AstBuildHelper.ReadLocalRV(tempDst)
                });
        else
            processedValue = AstBuildHelper.ReadLocalRV(origTempDst);

        if (op.ValuesPostProcessor != null)
        {
            var postProcessorId = AddObjectToStore(op.ValuesPostProcessor);
            processedValue = AstBuildHelper.CallMethod(
                op.ValuesPostProcessor.GetType().GetMethod("Invoke"),
                GetStoredObject(postProcessorId, op.ValuesPostProcessor.GetType()),
                new List<IAstStackItem>
                {
                    processedValue, AstBuildHelper.ReadLocalRV(LocState)
                });
        }

        copying.Add(
            AstBuildHelper.WriteMembersChain(
                op.Destination.MembersChain,
                AstBuildHelper.ReadLocalRA(LocTo),
                processedValue));

        if (ReflectionUtils.IsNullable(op.Source.MemberType) || !op.Source.MemberType.IsValueType)
            result.Nodes.Add(
                new AstIf
                {
                    Condition = ReflectionUtils.IsNullable(op.Source.MemberType)
                        ? new AstExprNot(
                            (IAstValue)AstBuildHelper.ReadPropertyRV(
                                AstBuildHelper.ReadLocalRA(origTempSrc),
                                op.Source.MemberType.GetProperty("HasValue")))
                        : new AstExprIsNull(AstBuildHelper.ReadLocalRV(origTempSrc)),
                    TrueBranch = new AstComplexNode
                    {
                        Nodes = writeNullToDest
                    },
                    FalseBranch = new AstComplexNode
                    {
                        Nodes = copying
                    }
                });
        else
            result.Nodes.AddRange(copying);
        return result;
    }

    private IAstNode Process_ReadWriteComplex_ByConverter(ReadWriteComplex op, int operationId)
    {
        IAstNode result;
        result = AstBuildHelper.WriteMembersChain(
            op.Destination.MembersChain,
            AstBuildHelper.ReadLocalRA(LocTo),
            AstBuildHelper.CallMethod(
                op.Converter.GetType().GetMethod("Invoke"),
                new AstCastclassRef(
                    (IAstRef)AstBuildHelper.ReadMemberRV(
                        GetStoredObject(operationId, typeof(ReadWriteComplex)),
                        typeof(ReadWriteComplex).GetProperty(nameof(ReadWriteComplex.Converter))),
                    op.Converter.GetType()),
                new List<IAstStackItem>
                {
                    AstBuildHelper.ReadMembersChain(AstBuildHelper.ReadLocalRA(LocFrom), op.Source.MembersChain),
                    AstBuildHelper.ReadLocalRV(LocState)
                }));
        return result;
    }

    private IAstRefOrValue GetNullValue(Delegate nullSubstitutor)
    {
        if (nullSubstitutor != null)
        {
            var substId = AddObjectToStore(nullSubstitutor);
            return AstBuildHelper.CallMethod(
                nullSubstitutor.GetType().GetMethod("Invoke"),
                GetStoredObject(substId, nullSubstitutor.GetType()),
                new List<IAstStackItem>
                {
                    AstBuildHelper.ReadLocalRV(LocState)
                });
        }

        return new AstConstantNull();
    }

    private int AddObjectToStore(object obj)
    {
        var objectId = StoredObjects.Count();
        StoredObjects.Add(obj);
        return objectId;
    }

    private IAstNode CreateExceptionHandlingBlock(int mappingItemId, IAstNode writeValue)
    {
        var handler = new AstThrow
        {
            Exception = new AstNewObject
            {
                ObjectType = typeof(EmitMapperException),
                ConstructorParams = new IAstStackItem[]
                {
                    new AstConstantString
                    {
                        Str = "Error in mapping operation execution: "
                    },
                    new AstReadLocalRef
                    {
                        LocalIndex = LocException.LocalIndex, LocalType = LocException.LocalType
                    },
                    GetStoredObject(mappingItemId, typeof(IMappingOperation))
                }
            }
        };

        var tryCatch = new AstExceptionHandlingBlock(
            writeValue,
            handler,
            typeof(Exception),
            LocException);
        return tryCatch;
    }

    private IAstNode WriteMappingValue(IMappingOperation mappingOperation, int mappingItemId, IAstRefOrValue value)
    {
        IAstNode writeValue;

        if (mappingOperation is SrcReadOperation srcReadOperation)
            writeValue = AstBuildHelper.CallMethod(
                typeof(ValueSetter).GetMethod(nameof(ValueSetter.Invoke)),
                new AstCastclassRef(
                    (IAstRef)AstBuildHelper.ReadMemberRV(
                        GetStoredObject(mappingItemId, typeof(SrcReadOperation)),
                        typeof(SrcReadOperation).GetProperty(nameof(SrcReadOperation.Setter))),
                    srcReadOperation.Setter.GetType()),
                new List<IAstStackItem>
                {
                    AstBuildHelper.ReadLocalRV(LocTo), value, AstBuildHelper.ReadLocalRV(LocState)
                });
        else
            writeValue = AstBuildHelper.WriteMembersChain(
                (mappingOperation as IDestOperation)?.Destination.MembersChain,
                AstBuildHelper.ReadLocalRA(LocTo),
                value);
        return writeValue;
    }

    private IAstRefOrValue ConvertMappingValue(ReadWriteSimple rwMapOp, int operationId, IAstRefOrValue sourceValue)
    {
        IAstRefOrValue convertedValue;
        if (rwMapOp.Converter != null)
        {
            convertedValue = AstBuildHelper.CallMethod(
                rwMapOp.Converter.GetType().GetMethod("Invoke"),
                new AstCastclassRef(
                    (IAstRef)AstBuildHelper.ReadMemberRV(
                        GetStoredObject(operationId, typeof(ReadWriteSimple)),
                        typeof(ReadWriteSimple).GetProperty(nameof(ReadWriteSimple.Converter))),
                    rwMapOp.Converter.GetType()),
                new List<IAstStackItem>
                {
                    sourceValue, AstBuildHelper.ReadLocalRV(LocState)
                });
        }
        else
        {
            if (rwMapOp.ShallowCopy && rwMapOp.Destination.MemberType == rwMapOp.Source.MemberType)
            {
                convertedValue = sourceValue;
            }
            else
            {
                var mi = StaticConvertersManager.GetStaticConverter(
                    rwMapOp.Source.MemberType,
                    rwMapOp.Destination.MemberType);
                if (mi != null)
                    convertedValue = AstBuildHelper.CallMethod(
                        mi,
                        null,
                        new List<IAstStackItem>
                        {
                            sourceValue
                        });
                else
                    convertedValue = ConvertByMapper(rwMapOp);
            }
        }

        return convertedValue;
    }

    private IAstRefOrValue ConvertByMapper(ReadWriteSimple mapping)
    {
        var mapper = ObjectsMapperManager.GetMapperInt(
            mapping.Source.MemberType,
            mapping.Destination.MemberType,
            MappingConfigurator);
        var mapperId = AddObjectToStore(mapper);

        var convertedValue = AstBuildHelper.CallMethod(
            typeof(ObjectsMapperBaseImpl).GetMethod(
                nameof(ObjectsMapperBaseImpl.Map),
                new[]
                {
                    typeof(object), typeof(object), typeof(object)
                }),
            new AstReadFieldRef
            {
                FieldInfo = typeof(ObjectsMapperDescr).GetField(nameof(ObjectsMapperDescr.Mapper)),
                SourceObject = GetStoredObject(mapperId, mapper.GetType())
            },
            new List<IAstStackItem>
            {
                AstBuildHelper.ReadMembersChain(AstBuildHelper.ReadLocalRA(LocFrom), mapping.Source.MembersChain),
                AstBuildHelper.ReadMembersChain(AstBuildHelper.ReadLocalRA(LocTo), mapping.Destination.MembersChain),
                (IAstRef)AstBuildHelper.ReadLocalRA(LocState)
            });
        return convertedValue;
    }

    private IAstNode ProcessDestSrcReadOperation(DestSrcReadOperation operation, int operationId)
    {
        var src = AstBuildHelper.ReadMembersChain(AstBuildHelper.ReadLocalRA(LocFrom), operation.Source.MembersChain);

        var dst = AstBuildHelper.ReadMembersChain(
            AstBuildHelper.ReadLocalRA(LocFrom),
            operation.Destination.MembersChain);

        return AstBuildHelper.CallMethod(
            typeof(ValueProcessor).GetMethod(nameof(ValueProcessor.Invoke)),
            new AstCastclassRef(
                (IAstRef)AstBuildHelper.ReadMemberRV(
                    GetStoredObject(operationId, typeof(DestSrcReadOperation)),
                    typeof(DestSrcReadOperation).GetProperty(nameof(DestSrcReadOperation.ValueProcessor))),
                operation.ValueProcessor.GetType()),
            new List<IAstStackItem>
            {
                src, dst, AstBuildHelper.ReadLocalRV(LocState)
            });
    }

    private IAstRefOrValue ReadSrcMappingValue(IMappingOperation mapping, int operationId)
    {
        if (mapping is ISrcReadOperation readOp)
            return AstBuildHelper.ReadMembersChain(AstBuildHelper.ReadLocalRA(LocFrom), readOp.Source.MembersChain);

        var destWriteOp = (DestWriteOperation)mapping;
        if (destWriteOp.Getter != null)
            return AstBuildHelper.CallMethod(
                destWriteOp.Getter.GetType().GetMethod("Invoke"),
                new AstCastclassRef(
                    (IAstRef)AstBuildHelper.ReadMemberRV(
                        GetStoredObject(operationId, typeof(DestWriteOperation)),
                        typeof(DestWriteOperation).GetProperty(nameof(DestWriteOperation.Getter))),
                    destWriteOp.Getter.GetType()),
                new List<IAstStackItem>
                {
                    AstBuildHelper.ReadLocalRV(LocState)
                });
        throw new EmitMapperException("Invalid mapping operations");
    }
}