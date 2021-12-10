namespace EmitMapper.EmitBuilders;

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

internal class MappingOperationsProcessor
{
    public CompilationContext compilationContext;

    public LocalBuilder locException;

    public LocalBuilder locFrom;

    public LocalBuilder locState;

    public LocalBuilder locTo;

    public IMappingConfigurator mappingConfigurator;

    public ObjectMapperManager objectsMapperManager;

    public IEnumerable<IMappingOperation> operations = new List<IMappingOperation>();

    public IRootMappingOperation rootOperation;

    public StaticConvertersManager staticConvertersManager;

    public List<object> storedObjects = new();

    public MappingOperationsProcessor()
    {
    }

    public MappingOperationsProcessor(MappingOperationsProcessor prototype)
    {
        this.locFrom = prototype.locFrom;
        this.locTo = prototype.locTo;
        this.locState = prototype.locState;
        this.locException = prototype.locException;
        this.compilationContext = prototype.compilationContext;
        this.operations = prototype.operations;
        this.storedObjects = prototype.storedObjects;
        this.mappingConfigurator = prototype.mappingConfigurator;
        this.objectsMapperManager = prototype.objectsMapperManager;
        this.rootOperation = prototype.rootOperation;
        this.staticConvertersManager = prototype.staticConvertersManager;
    }

    public IAstNode ProcessOperations()
    {
        var result = new AstComplexNode();
        foreach (var operation in this.operations)
        {
            IAstNode completeOperation = null;
            var operationId = this.AddObjectToStore(operation);

            if (operation is OperationsBlock)
                completeOperation =
                    new MappingOperationsProcessor(this) { operations = (operation as OperationsBlock).Operations }
                        .ProcessOperations();
            else if (operation is ReadWriteComplex)
                completeOperation = this.Process_ReadWriteComplex(operation as ReadWriteComplex, operationId);
            else if (operation is DestSrcReadOperation)
                completeOperation = this.ProcessDestSrcReadOperation(operation as DestSrcReadOperation, operationId);
            else if (operation is SrcReadOperation)
                completeOperation = this.ProcessSrcReadOperation(operation as SrcReadOperation, operationId);
            else if (operation is DestWriteOperation)
                completeOperation = this.ProcessDestWriteOperation(operation as DestWriteOperation, operationId);
            else if (operation is ReadWriteSimple)
                completeOperation = this.ProcessReadWriteSimple(operation as ReadWriteSimple, operationId);

            if (completeOperation == null)
                continue;
            if (this.locException != null)
            {
                var tryCatch = this.CreateExceptionHandlingBlock(operationId, completeOperation);
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
        var sourceValue = this.ReadSrcMappingValue(readWriteSimple, operationId);

        IAstRefOrValue convertedValue;

        if (readWriteSimple.NullSubstitutor != null && (ReflectionUtils.IsNullable(readWriteSimple.Source.MemberType)
                                                        || !readWriteSimple.Source.MemberType.IsValueType))
            convertedValue = new AstIfTernar(
                ReflectionUtils.IsNullable(readWriteSimple.Source.MemberType)
                    ? new AstExprNot(
                        AstBuildHelper.ReadPropertyRV(
                            new AstValueToAddr((IAstValue)sourceValue),
                            readWriteSimple.Source.MemberType.GetProperty("HasValue")))
                    : new AstExprIsNull(sourceValue),
                this.GetNullValue(readWriteSimple.NullSubstitutor), // source is null
                AstBuildHelper.CastClass(
                    this.ConvertMappingValue(readWriteSimple, operationId, sourceValue),
                    readWriteSimple.Destination.MemberType));
        else
            convertedValue = this.ConvertMappingValue(readWriteSimple, operationId, sourceValue);

        var result = this.WriteMappingValue(readWriteSimple, operationId, convertedValue);

        if (readWriteSimple.SourceFilter != null)
            result = this.Process_SourceFilter(readWriteSimple, operationId, result);

        if (readWriteSimple.DestinationFilter != null)
            result = this.Process_DestinationFilter(readWriteSimple, operationId, result);
        return result;
    }

    private IAstNode ProcessDestWriteOperation(DestWriteOperation destWriteOperation, int operationId)
    {
        LocalBuilder locValueToWrite = null;
        locValueToWrite = this.compilationContext.ILGenerator.DeclareLocal(destWriteOperation.Getter.Method.ReturnType);

        var cmdValue = new AstWriteLocal(
            locValueToWrite,
            AstBuildHelper.CallMethod(
                destWriteOperation.Getter.GetType().GetMethod("Invoke"),
                new AstCastclassRef(
                    (IAstRef)AstBuildHelper.ReadMemberRV(
                        GetStoredObject(operationId, typeof(DestWriteOperation)),
                        typeof(DestWriteOperation).GetProperty("Getter")),
                    destWriteOperation.Getter.GetType()),
                new List<IAstStackItem>
                    {
                        AstBuildHelper.ReadLocalRV(this.locFrom), AstBuildHelper.ReadLocalRV(this.locState)
                    }));

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
                                                               (MemberInfo)locValueToWrite.LocalType.GetField("action")
                                                           }),
                                                   new AstConstantInt32 { Value = 0 }),
                                               TrueBranch = new AstComplexNode
                                                                {
                                                                    Nodes = new List<IAstNode>
                                                                                {
                                                                                    AstBuildHelper.WriteMembersChain(
                                                                                        destWriteOperation.Destination
                                                                                            .MembersChain,
                                                                                        AstBuildHelper.ReadLocalRA(
                                                                                            this.locTo),
                                                                                        AstBuildHelper.ReadMembersChain(
                                                                                            AstBuildHelper.ReadLocalRA(
                                                                                                locValueToWrite),
                                                                                            new[]
                                                                                                {
                                                                                                    (MemberInfo)
                                                                                                    locValueToWrite
                                                                                                        .LocalType
                                                                                                        .GetField(
                                                                                                            "value")
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
            AstBuildHelper.ReadLocalRA(this.locFrom),
            srcReadOperation.Source.MembersChain);

        return this.WriteMappingValue(srcReadOperation, operationId, value);
    }

    private IAstNode Process_ReadWriteComplex(ReadWriteComplex op, int operationId)
    {
        IAstNode result;
        if (op.Converter != null)
            result = this.Process_ReadWriteComplex_ByConverter(op, operationId);
        else
            result = this.Process_ReadWriteComplex_Copying(op);

        if (op.SourceFilter != null)
            result = this.Process_SourceFilter(op, operationId, result);

        if (op.DestinationFilter != null)
            result = this.Process_DestinationFilter(op, operationId, result);

        return result;
    }

    private IAstNode Process_DestinationFilter(IReadWriteOperation op, int operationId, IAstNode result)
    {
        return this.Process_ValuesFilter(
            op,
            operationId,
            result,
            AstBuildHelper.ReadMembersChain(AstBuildHelper.ReadLocalRA(this.locTo), op.Destination.MembersChain),
            "DestinationFilter",
            op.DestinationFilter);
    }

    private IAstNode Process_SourceFilter(IReadWriteOperation op, int operationId, IAstNode result)
    {
        return this.Process_ValuesFilter(
            op,
            operationId,
            result,
            AstBuildHelper.ReadMembersChain(AstBuildHelper.ReadLocalRA(this.locFrom), op.Source.MembersChain),
            "SourceFilter",
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
                                                             value, AstBuildHelper.ReadLocalRV(this.locState)
                                                         }),
                                                 TrueBranch =
                                                     new AstComplexNode { Nodes = new List<IAstNode> { result } }
                                             }
                                     }
                     };
        return result;
    }

    private IAstNode Process_ReadWriteComplex_Copying(ReadWriteComplex op)
    {
        var result = new AstComplexNode();
        LocalBuilder origTempSrc, origTempDst;
        var tempSrc = this.compilationContext.ILGenerator.DeclareLocal(op.Source.MemberType);
        var tempDst = this.compilationContext.ILGenerator.DeclareLocal(op.Destination.MemberType);
        origTempSrc = tempSrc;
        origTempDst = tempDst;

        result.Nodes.Add(
            new AstWriteLocal(
                tempSrc,
                AstBuildHelper.ReadMembersChain(AstBuildHelper.ReadLocalRA(this.locFrom), op.Source.MembersChain)));
        result.Nodes.Add(
            new AstWriteLocal(
                tempDst,
                AstBuildHelper.ReadMembersChain(AstBuildHelper.ReadLocalRA(this.locTo), op.Destination.MembersChain)));

        var writeNullToDest = new List<IAstNode>
                                  {
                                      AstBuildHelper.WriteMembersChain(
                                          op.Destination.MembersChain,
                                          AstBuildHelper.ReadLocalRA(this.locTo),
                                          this.GetNullValue(op.NullSubstitutor))
                                  };

        // Target construction

        var initDest = new List<IAstNode>();
        var custCtr = op.TargetConstructor;
        if (custCtr != null)
        {
            var custCtrIdx = this.AddObjectToStore(custCtr);
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
            tempSrc = this.compilationContext.ILGenerator.DeclareLocal(
                Nullable.GetUnderlyingType(op.Source.MemberType));
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
                        TrueBranch = new AstComplexNode { Nodes = initDest }
                    });
            if (ReflectionUtils.IsNullable(op.Destination.MemberType))
            {
                tempDst = this.compilationContext.ILGenerator.DeclareLocal(
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
                                            operations = op.Operations,
                                            locTo = tempDst,
                                            locFrom = tempSrc,
                                            rootOperation = this.mappingConfigurator.GetRootMappingOperation(
                                                op.Source.MemberType,
                                                op.Destination.MemberType)
                                        }.ProcessOperations()
                                }
                });

        IAstRefOrValue processedValue;
        if (ReflectionUtils.IsNullable(op.Destination.MemberType))
            processedValue = new AstNewObject(op.Destination.MemberType, new[] { AstBuildHelper.ReadLocalRV(tempDst) });
        else
            processedValue = AstBuildHelper.ReadLocalRV(origTempDst);

        if (op.ValuesPostProcessor != null)
        {
            var postProcessorId = this.AddObjectToStore(op.ValuesPostProcessor);
            processedValue = AstBuildHelper.CallMethod(
                op.ValuesPostProcessor.GetType().GetMethod("Invoke"),
                GetStoredObject(postProcessorId, op.ValuesPostProcessor.GetType()),
                new List<IAstStackItem> { processedValue, AstBuildHelper.ReadLocalRV(this.locState) });
        }

        copying.Add(
            AstBuildHelper.WriteMembersChain(
                op.Destination.MembersChain,
                AstBuildHelper.ReadLocalRA(this.locTo),
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
                        TrueBranch = new AstComplexNode { Nodes = writeNullToDest },
                        FalseBranch = new AstComplexNode { Nodes = copying }
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
            AstBuildHelper.ReadLocalRA(this.locTo),
            AstBuildHelper.CallMethod(
                op.Converter.GetType().GetMethod("Invoke"),
                new AstCastclassRef(
                    (IAstRef)AstBuildHelper.ReadMemberRV(
                        GetStoredObject(operationId, typeof(ReadWriteComplex)),
                        typeof(ReadWriteComplex).GetProperty("Converter")),
                    op.Converter.GetType()),
                new List<IAstStackItem>
                    {
                        AstBuildHelper.ReadMembersChain(
                            AstBuildHelper.ReadLocalRA(this.locFrom),
                            op.Source.MembersChain),
                        AstBuildHelper.ReadLocalRV(this.locState)
                    }));
        return result;
    }

    private IAstRefOrValue GetNullValue(Delegate nullSubstitutor)
    {
        if (nullSubstitutor != null)
        {
            var substId = this.AddObjectToStore(nullSubstitutor);
            return AstBuildHelper.CallMethod(
                nullSubstitutor.GetType().GetMethod("Invoke"),
                GetStoredObject(substId, nullSubstitutor.GetType()),
                new List<IAstStackItem> { AstBuildHelper.ReadLocalRV(this.locState) });
        }

        return new AstConstantNull();
    }

    private int AddObjectToStore(object obj)
    {
        var objectId = this.storedObjects.Count();
        this.storedObjects.Add(obj);
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
                                                                                      Str =
                                                                                          "Error in mapping operation execution: "
                                                                                  },
                                                                              new AstReadLocalRef
                                                                                  {
                                                                                      LocalIndex =
                                                                                          this.locException.LocalIndex,
                                                                                      LocalType = this.locException
                                                                                          .LocalType
                                                                                  },
                                                                              GetStoredObject(
                                                                                  mappingItemId,
                                                                                  typeof(IMappingOperation))
                                                                          }
                                              }
                          };

        var tryCatch = new AstExceptionHandlingBlock(writeValue, handler, typeof(Exception), this.locException);
        return tryCatch;
    }

    private IAstNode WriteMappingValue(IMappingOperation mappingOperation, int mappingItemId, IAstRefOrValue value)
    {
        IAstNode writeValue;

        if (mappingOperation is SrcReadOperation)
            writeValue = AstBuildHelper.CallMethod(
                typeof(ValueSetter).GetMethod("Invoke"),
                new AstCastclassRef(
                    (IAstRef)AstBuildHelper.ReadMemberRV(
                        GetStoredObject(mappingItemId, typeof(SrcReadOperation)),
                        typeof(SrcReadOperation).GetProperty("Setter")),
                    (mappingOperation as SrcReadOperation).Setter.GetType()),
                new List<IAstStackItem>
                    {
                        AstBuildHelper.ReadLocalRV(this.locTo), value, AstBuildHelper.ReadLocalRV(this.locState)
                    });
        else
            writeValue = AstBuildHelper.WriteMembersChain(
                (mappingOperation as IDestOperation).Destination.MembersChain,
                AstBuildHelper.ReadLocalRA(this.locTo),
                value);
        return writeValue;
    }

    private IAstRefOrValue ConvertMappingValue(ReadWriteSimple rwMapOp, int operationId, IAstRefOrValue sourceValue)
    {
        var convertedValue = sourceValue;
        if (rwMapOp.Converter != null)
        {
            convertedValue = AstBuildHelper.CallMethod(
                rwMapOp.Converter.GetType().GetMethod("Invoke"),
                new AstCastclassRef(
                    (IAstRef)AstBuildHelper.ReadMemberRV(
                        GetStoredObject(operationId, typeof(ReadWriteSimple)),
                        typeof(ReadWriteSimple).GetProperty("Converter")),
                    rwMapOp.Converter.GetType()),
                new List<IAstStackItem> { sourceValue, AstBuildHelper.ReadLocalRV(this.locState) });
        }
        else
        {
            if (rwMapOp.ShallowCopy && rwMapOp.Destination.MemberType == rwMapOp.Source.MemberType)
            {
                convertedValue = sourceValue;
            }
            else
            {
                var mi = this.staticConvertersManager.GetStaticConverter(
                    rwMapOp.Source.MemberType,
                    rwMapOp.Destination.MemberType);
                if (mi != null)
                    convertedValue = AstBuildHelper.CallMethod(mi, null, new List<IAstStackItem> { sourceValue });
                else
                    convertedValue = this.ConvertByMapper(rwMapOp);
            }
        }

        return convertedValue;
    }

    private IAstRefOrValue ConvertByMapper(ReadWriteSimple mapping)
    {
        IAstRefOrValue convertedValue;
        var mapper = this.objectsMapperManager.GetMapperInt(
            mapping.Source.MemberType,
            mapping.Destination.MemberType,
            this.mappingConfigurator);
        var mapperId = this.AddObjectToStore(mapper);

        convertedValue = AstBuildHelper.CallMethod(
            typeof(ObjectsMapperBaseImpl).GetMethod("Map", new[] { typeof(object), typeof(object), typeof(object) }),
            new AstReadFieldRef
                {
                    FieldInfo = typeof(ObjectsMapperDescr).GetField("mapper"),
                    SourceObject = GetStoredObject(mapperId, mapper.GetType())
                },
            new List<IAstStackItem>
                {
                    AstBuildHelper.ReadMembersChain(
                        AstBuildHelper.ReadLocalRA(this.locFrom),
                        mapping.Source.MembersChain),
                    AstBuildHelper.ReadMembersChain(
                        AstBuildHelper.ReadLocalRA(this.locTo),
                        mapping.Destination.MembersChain),
                    (IAstRef)AstBuildHelper.ReadLocalRA(this.locState)
                });
        return convertedValue;
    }

    private IAstNode ProcessDestSrcReadOperation(DestSrcReadOperation operation, int operationId)
    {
        var src = AstBuildHelper.ReadMembersChain(
            AstBuildHelper.ReadLocalRA(this.locFrom),
            operation.Source.MembersChain);

        var dst = AstBuildHelper.ReadMembersChain(
            AstBuildHelper.ReadLocalRA(this.locFrom),
            operation.Destination.MembersChain);

        return AstBuildHelper.CallMethod(
            typeof(ValueProcessor).GetMethod("Invoke"),
            new AstCastclassRef(
                (IAstRef)AstBuildHelper.ReadMemberRV(
                    GetStoredObject(operationId, typeof(DestSrcReadOperation)),
                    typeof(DestWriteOperation).GetProperty("ValueProcessor")),
                operation.ValueProcessor.GetType()),
            new List<IAstStackItem> { src, dst, AstBuildHelper.ReadLocalRV(this.locState) });
    }

    private IAstRefOrValue ReadSrcMappingValue(IMappingOperation mapping, int operationId)
    {
        var readOp = mapping as ISrcReadOperation;
        if (readOp != null)
            return AstBuildHelper.ReadMembersChain(
                AstBuildHelper.ReadLocalRA(this.locFrom),
                readOp.Source.MembersChain);

        var destWriteOp = (DestWriteOperation)mapping;
        if (destWriteOp.Getter != null)
            return AstBuildHelper.CallMethod(
                destWriteOp.Getter.GetType().GetMethod("Invoke"),
                new AstCastclassRef(
                    (IAstRef)AstBuildHelper.ReadMemberRV(
                        GetStoredObject(operationId, typeof(DestWriteOperation)),
                        typeof(DestWriteOperation).GetProperty("Getter")),
                    destWriteOp.Getter.GetType()),
                new List<IAstStackItem> { AstBuildHelper.ReadLocalRV(this.locState) });
        throw new EmitMapperException("Invalid mapping operations");
    }

    private static IAstRef GetStoredObject(int objectIndex, Type castType)
    {
        var result = (IAstRef)AstBuildHelper.ReadArrayItemRV(
            (IAstRef)AstBuildHelper.ReadFieldRA(
                new AstReadThis { ThisType = typeof(ObjectsMapperBaseImpl) },
                typeof(ObjectsMapperBaseImpl).GetField("StroredObjects", BindingFlags.Instance | BindingFlags.Public)),
            objectIndex);
        if (castType != null)
            result = new AstCastclassRef(result, castType);
        return result;
    }
}