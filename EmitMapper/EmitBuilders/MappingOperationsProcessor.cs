using EmitMapper.AST;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using EmitMapper.AST.Nodes;
using EmitMapper.Conversion;
using EmitMapper.Mappers;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace EmitMapper.EmitBuilders
{

    /* Unmerged change from project 'EmitMapper (netstandard2.1)'
    Before:
        class MappingOperationsProcessor
    After:
        class MappingOperationsProcessor
    */
    internal class MappingOperationsProcessor
    {
        public LocalBuilder locFrom;
        public LocalBuilder locTo;
        public LocalBuilder locState;
        public LocalBuilder locException;
        public CompilationContext compilationContext;
        public IEnumerable<IMappingOperation> operations = new List<IMappingOperation>();
        public List<object> storedObjects = new List<object>();
        public IMappingConfigurator mappingConfigurator;
        public ObjectMapperManager objectsMapperManager;
        public IRootMappingOperation rootOperation;
        public StaticConvertersManager staticConvertersManager;

        public MappingOperationsProcessor()
        {
        }

        public MappingOperationsProcessor(MappingOperationsProcessor prototype)
        {
            locFrom = prototype.locFrom;
            locTo = prototype.locTo;
            locState = prototype.locState;
            locException = prototype.locException;
            compilationContext = prototype.compilationContext;
            operations = prototype.operations;
            storedObjects = prototype.storedObjects;
            mappingConfigurator = prototype.mappingConfigurator;
            objectsMapperManager = prototype.objectsMapperManager;
            rootOperation = prototype.rootOperation;
            staticConvertersManager = prototype.staticConvertersManager;
        }

        public IAstNode ProcessOperations()
        {

            /* Unmerged change from project 'EmitMapper (netstandard2.1)'
            Before:
                        var result = new AstComplexNode();
            After:
                        var result = new AstComplexNode();
            */
            AstComplexNode result = new AstComplexNode();
            foreach (IMappingOperation operation in operations)
            {
                IAstNode completeOperation = null;
                int operationId = AddObjectToStore(operation);

                if (operation is OperationsBlock)
                {
                    completeOperation =
                        new MappingOperationsProcessor(this)
                        {
                            operations = (operation as OperationsBlock).Operations
                        }.ProcessOperations();
                }
                else if (operation is ReadWriteComplex)
                {
                    completeOperation = Process_ReadWriteComplex(operation as ReadWriteComplex, operationId);
                }
                else if (operation is DestSrcReadOperation)
                {
                    completeOperation = ProcessDestSrcReadOperation(operation as DestSrcReadOperation, operationId);
                }
                else if (operation is SrcReadOperation)
                {
                    completeOperation = ProcessSrcReadOperation(operation as SrcReadOperation, operationId);
                }
                else if (operation is DestWriteOperation)
                {
                    completeOperation = ProcessDestWriteOperation(operation as DestWriteOperation, operationId);
                }
                else if (operation is ReadWriteSimple)
                {
                    completeOperation = ProcessReadWriteSimple(operation as ReadWriteSimple, operationId);
                }

                if (completeOperation == null)
                {
                    continue;
                }
                if (locException != null)
                {

                    /* Unmerged change from project 'EmitMapper (netstandard2.1)'
                    Before:
                                        var tryCatch = CreateExceptionHandlingBlock(operationId, completeOperation);
                    After:
                                        var tryCatch = CreateExceptionHandlingBlock(operationId, completeOperation);
                    */
                    IAstNode tryCatch = CreateExceptionHandlingBlock(operationId, completeOperation);
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
            IAstRefOrValue sourceValue = ReadSrcMappingValue(readWriteSimple, operationId);

            IAstRefOrValue convertedValue;

            if (readWriteSimple.NullSubstitutor != null && (ReflectionUtils.IsNullable(readWriteSimple.Source.MemberType) || !readWriteSimple.Source.MemberType.IsValueType))
            {
                convertedValue = new AstIfTernar(
                    ReflectionUtils.IsNullable(readWriteSimple.Source.MemberType)
                        ? new AstExprNot(AstBuildHelper.ReadPropertyRV(new AstValueToAddr((IAstValue)sourceValue), readWriteSimple.Source.MemberType.GetProperty("HasValue")))
                        : new AstExprIsNull(sourceValue),
                        GetNullValue(readWriteSimple.NullSubstitutor), // source is null
                        AstBuildHelper.CastClass(
                            ConvertMappingValue(
                                readWriteSimple,
                                operationId,
                                sourceValue
                            ),
                            readWriteSimple.Destination.MemberType
                        )
                );
            }
            else
            {
                convertedValue =
                    ConvertMappingValue(
                        readWriteSimple,
                        operationId,
                        sourceValue
                    );
            }

            IAstNode result = WriteMappingValue(readWriteSimple, operationId, convertedValue);

            if (readWriteSimple.SourceFilter != null)
            {
                result = Process_SourceFilter(readWriteSimple, operationId, result);
            }

            if (readWriteSimple.DestinationFilter != null)
            {
                result = Process_DestinationFilter(readWriteSimple, operationId, result);
            }
            return result;
        }

        private IAstNode ProcessDestWriteOperation(DestWriteOperation destWriteOperation, int operationId)
        {
            LocalBuilder locValueToWrite = null;
            locValueToWrite = compilationContext.ILGenerator.DeclareLocal(destWriteOperation.Getter.Method.ReturnType);


            /* Unmerged change from project 'EmitMapper (netstandard2.1)'
            Before:
                        var cmdValue = new AstWriteLocal(
            After:
                        var cmdValue = new AstWriteLocal(
            */
            AstWriteLocal cmdValue = new AstWriteLocal(
                locValueToWrite,
                AstBuildHelper.CallMethod(
                    destWriteOperation.Getter.GetType().GetMethod("Invoke"),
                    new AstCastclassRef(
                        (IAstRef)AstBuildHelper.ReadMemberRV(
                            GetStoredObject(operationId, typeof(DestWriteOperation)),
                            typeof(DestWriteOperation).GetProperty("Getter")
                        ),
                        destWriteOperation.Getter.GetType()
                    ),
                    new List<IAstStackItem>
                    {
                        AstBuildHelper.ReadLocalRV(locFrom),
                        AstBuildHelper.ReadLocalRV(locState)
                    }
                )
            );

            return
                new AstComplexNode
                {
                    Nodes = new List<IAstNode>
                    {
                        cmdValue,
                        new AstIf()
                        {
                            Condition = new AstExprEquals(
                                (IAstValue)AstBuildHelper.ReadMembersChain(
                                    AstBuildHelper.ReadLocalRA(locValueToWrite),
                                    new[] { (MemberInfo)locValueToWrite.LocalType.GetField("action") }
                                ),
                                new AstConstantInt32() { Value = 0 }
                            ),
                            TrueBranch = new AstComplexNode
                            {
                                Nodes = new List<IAstNode>
                                {
                                     AstBuildHelper.WriteMembersChain(
                                        destWriteOperation.Destination.MembersChain,
                                        AstBuildHelper.ReadLocalRA(locTo),
                                        AstBuildHelper.ReadMembersChain(
                                            AstBuildHelper.ReadLocalRA(locValueToWrite),
                                            new[] { (MemberInfo)locValueToWrite.LocalType.GetField("value") }
                                        )
                                    )
                                }
                            }
                        }
                    }
                };
        }

        private IAstNode ProcessSrcReadOperation(SrcReadOperation srcReadOperation, int operationId)
        {

            /* Unmerged change from project 'EmitMapper (netstandard2.1)'
            Before:
                        var value = AstBuildHelper.ReadMembersChain(
            After:
                        var value = AstBuildHelper.ReadMembersChain(
            */
            IAstRefOrValue value = AstBuildHelper.ReadMembersChain(
                AstBuildHelper.ReadLocalRA(locFrom),
                srcReadOperation.Source.MembersChain
            );

            return WriteMappingValue(srcReadOperation, operationId, value);
        }

        private IAstNode Process_ReadWriteComplex(ReadWriteComplex op, int operationId)
        {
            IAstNode result;
            if (op.Converter != null)
            {
                result = Process_ReadWriteComplex_ByConverter(op, operationId);
            }
            else
            {
                result = Process_ReadWriteComplex_Copying(op);
            }

            if (op.SourceFilter != null)
            {
                result = Process_SourceFilter(op, operationId, result);
            }

            if (op.DestinationFilter != null)
            {
                result = Process_DestinationFilter(op, operationId, result);
            }

            return result;
        }

        private IAstNode Process_DestinationFilter(IReadWriteOperation op, int operationId, IAstNode result)
        {
            return Process_ValuesFilter(
                op,
                operationId,
                result,
                AstBuildHelper.ReadMembersChain(
                    AstBuildHelper.ReadLocalRA(locTo),
                    op.Destination.MembersChain
                ),
                "DestinationFilter",
                op.DestinationFilter
            );
        }

        private IAstNode Process_SourceFilter(IReadWriteOperation op, int operationId, IAstNode result)
        {
            return Process_ValuesFilter(
                op,
                operationId,
                result,
                AstBuildHelper.ReadMembersChain(
                    AstBuildHelper.ReadLocalRA(locFrom),
                    op.Source.MembersChain
                ),
                "SourceFilter",
                op.SourceFilter
            );
        }
        private IAstNode Process_ValuesFilter(
            IReadWriteOperation op,
            int operationId,
            IAstNode result,
            IAstRefOrValue value,
            string fieldName,
            Delegate filterDelegate
        )
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
                                         typeof(IReadWriteOperation).GetProperty(fieldName)
                                    ),
                                    filterDelegate.GetType()
                                ),
                                new List<IAstStackItem>()
                                {
                                    value,
                                    AstBuildHelper.ReadLocalRV(locState),
                                }
                            ),
                            TrueBranch = new AstComplexNode{ Nodes = new List<IAstNode> { result } }
                        }
                    }
            };
            return result;
        }

        private IAstNode Process_ReadWriteComplex_Copying(ReadWriteComplex op)
        {

            /* Unmerged change from project 'EmitMapper (netstandard2.1)'
            Before:
                        var result = new AstComplexNode();
            After:
                        var result = new AstComplexNode();
            */
            AstComplexNode result = new AstComplexNode();
            LocalBuilder origTempSrc, origTempDst;
            LocalBuilder tempSrc = compilationContext.ILGenerator.DeclareLocal(op.Source.MemberType);
            LocalBuilder tempDst = compilationContext.ILGenerator.DeclareLocal(op.Destination.MemberType);
            origTempSrc = tempSrc;
            origTempDst = tempDst;

            result.Nodes.Add(
                new AstWriteLocal(tempSrc, AstBuildHelper.ReadMembersChain(AstBuildHelper.ReadLocalRA(locFrom), op.Source.MembersChain)
                )
            );
            result.Nodes.Add(
                new AstWriteLocal(tempDst, AstBuildHelper.ReadMembersChain(AstBuildHelper.ReadLocalRA(locTo), op.Destination.MembersChain))
            );


            /* Unmerged change from project 'EmitMapper (netstandard2.1)'
            Before:
                        var writeNullToDest =
            After:
                        var writeNullToDest =
            */
            List<IAstNode> writeNullToDest =
                new List<IAstNode>
                {
                    AstBuildHelper.WriteMembersChain(
                        op.Destination.MembersChain,
                        AstBuildHelper.ReadLocalRA(locTo),
                        GetNullValue(op.NullSubstitutor)
                    )
                };

            // Target construction

            /* Unmerged change from project 'EmitMapper (netstandard2.1)'
            Before:
                        var initDest = new List<IAstNode>();
                        var custCtr = op.TargetConstructor;
            After:
                        var initDest = new List<IAstNode>();
                        var custCtr = op.TargetConstructor;
            */
            List<IAstNode> initDest = new List<IAstNode>();
            Delegate custCtr = op.TargetConstructor;
            if (custCtr != null)
            {
                int custCtrIdx = AddObjectToStore(custCtr);
                initDest.Add(
                    new AstWriteLocal(
                        tempDst,
                        AstBuildHelper.CallMethod(
                            custCtr.GetType().GetMethod("Invoke"),
                            GetStoredObject(custCtrIdx, custCtr.GetType()),
                            null
                        )
                    )
                );
            }
            else
            {
                initDest.Add(
                    new AstWriteLocal(tempDst, new AstNewObject(op.Destination.MemberType, null))
                );
            }


            /* Unmerged change from project 'EmitMapper (netstandard2.1)'
            Before:
                        var copying = new List<IAstNode>();
            After:
                        var copying = new List<IAstNode>();
            */
            List<IAstNode> copying = new List<IAstNode>();

            // if destination is nullable, create a temp target variable with underlying destination type
            if (ReflectionUtils.IsNullable(op.Source.MemberType))
            {
                tempSrc = compilationContext.ILGenerator.DeclareLocal(Nullable.GetUnderlyingType(op.Source.MemberType));
                copying.Add(
                    new AstWriteLocal(
                        tempSrc,
                        AstBuildHelper.ReadPropertyRV(
                            AstBuildHelper.ReadLocalRA(origTempSrc),
                            op.Source.MemberType.GetProperty("Value")
                        )
                    )
                );
            }

            // If destination is null, initialize it.
            if (ReflectionUtils.IsNullable(op.Destination.MemberType) || !op.Destination.MemberType.IsValueType)
            {
                copying.Add(
                    new AstIf()
                    {
                        Condition = ReflectionUtils.IsNullable(op.Destination.MemberType)
                            ? new AstExprNot((IAstValue)AstBuildHelper.ReadPropertyRV(AstBuildHelper.ReadLocalRA(origTempDst), op.Destination.MemberType.GetProperty("HasValue")))
                            : new AstExprIsNull(AstBuildHelper.ReadLocalRV(origTempDst)),
                        TrueBranch = new AstComplexNode() { Nodes = initDest }
                    }
                );
                if (ReflectionUtils.IsNullable(op.Destination.MemberType))
                {
                    tempDst = compilationContext.ILGenerator.DeclareLocal(Nullable.GetUnderlyingType(op.Destination.MemberType));
                    copying.Add(
                        new AstWriteLocal(
                            tempDst,
                            AstBuildHelper.ReadPropertyRV(
                                AstBuildHelper.ReadLocalRA(origTempDst),
                                op.Destination.MemberType.GetProperty("Value")
                            )
                        )
                    );
                }
            }

            // Suboperations
            copying.Add(
                new AstComplexNode()
                {
                    Nodes = new List<IAstNode>
                    {
                        new MappingOperationsProcessor(this)
                        {
                            operations = op.Operations,
                            locTo = tempDst,
                            locFrom = tempSrc,
                            rootOperation = mappingConfigurator.GetRootMappingOperation(op.Source.MemberType, op.Destination.MemberType)
                        }.ProcessOperations()
                    }
                }
            );

            IAstRefOrValue processedValue;
            if (ReflectionUtils.IsNullable(op.Destination.MemberType))
            {
                processedValue =
                    new AstNewObject(
                        op.Destination.MemberType,
                        new[]
                        {
                            AstBuildHelper.ReadLocalRV(tempDst)
                        }
                    );
            }
            else
            {
                processedValue = AstBuildHelper.ReadLocalRV(origTempDst);
            }

            if (op.ValuesPostProcessor != null)
            {
                int postProcessorId = AddObjectToStore(op.ValuesPostProcessor);
                processedValue =
                    AstBuildHelper.CallMethod(
                        op.ValuesPostProcessor.GetType().GetMethod("Invoke"),
                        GetStoredObject(postProcessorId, op.ValuesPostProcessor.GetType()),
                        new List<IAstStackItem>
                        {
                            processedValue,
                            AstBuildHelper.ReadLocalRV(locState)
                        }
                    );
            }

            copying.Add(
                AstBuildHelper.WriteMembersChain(
                    op.Destination.MembersChain,
                    AstBuildHelper.ReadLocalRA(locTo),
                    processedValue
                )
            );

            if (ReflectionUtils.IsNullable(op.Source.MemberType) || !op.Source.MemberType.IsValueType)
            {
                result.Nodes.Add(
                    new AstIf()
                    {
                        Condition = ReflectionUtils.IsNullable(op.Source.MemberType)
                            ? new AstExprNot((IAstValue)AstBuildHelper.ReadPropertyRV(AstBuildHelper.ReadLocalRA(origTempSrc), op.Source.MemberType.GetProperty("HasValue")))
                            : new AstExprIsNull(AstBuildHelper.ReadLocalRV(origTempSrc)),
                        TrueBranch = new AstComplexNode() { Nodes = writeNullToDest },
                        FalseBranch = new AstComplexNode() { Nodes = copying }
                    }
                );
            }
            else
            {
                result.Nodes.AddRange(copying);
            }
            return result;
        }

        private IAstNode Process_ReadWriteComplex_ByConverter(ReadWriteComplex op, int operationId)
        {
            IAstNode result;
            result =
                AstBuildHelper.WriteMembersChain(
                    op.Destination.MembersChain,
                    AstBuildHelper.ReadLocalRA(locTo),
                    AstBuildHelper.CallMethod(
                        op.Converter.GetType().GetMethod("Invoke"),
                        new AstCastclassRef(
                            (IAstRef)AstBuildHelper.ReadMemberRV(
                                 GetStoredObject(operationId, typeof(ReadWriteComplex)),
                                 typeof(ReadWriteComplex).GetProperty("Converter")
                            ),
                            op.Converter.GetType()
                        ),
                        new List<IAstStackItem>()
                        {
                            AstBuildHelper.ReadMembersChain(
                                AstBuildHelper.ReadLocalRA(locFrom),
                                op.Source.MembersChain
                            ),
                            AstBuildHelper.ReadLocalRV(locState),
                        }
                    )
                );
            return result;
        }

        private IAstRefOrValue GetNullValue(Delegate nullSubstitutor)
        {
            if (nullSubstitutor != null)
            {
                int substId = AddObjectToStore(nullSubstitutor);
                return
                    AstBuildHelper.CallMethod(
                        nullSubstitutor.GetType().GetMethod("Invoke"),
                        GetStoredObject(substId, nullSubstitutor.GetType()),
                        new List<IAstStackItem>
                        {
                            AstBuildHelper.ReadLocalRV(locState)
                        }
                    );
            }
            else
            {
                return new AstConstantNull();
            }
        }
        private int AddObjectToStore(object obj)
        {
            int objectId = storedObjects.Count();
            storedObjects.Add(obj);
            return objectId;
        }

        private IAstNode CreateExceptionHandlingBlock(int mappingItemId, IAstNode writeValue)
        {

            /* Unmerged change from project 'EmitMapper (netstandard2.1)'
            Before:
                        var handler =
            After:
                        var handler =
            */
            AstThrow handler =
                new AstThrow
                {
                    Exception =
                        new AstNewObject
                        {
                            ObjectType = typeof(EmitMapperException),
                            ConstructorParams =
                                new IAstStackItem[]
                                {
                                    new AstConstantString()
                                    {
                                        Str = "Error in mapping operation execution: "
                                    },
                                    new AstReadLocalRef()
                                    {
                                        LocalIndex = locException.LocalIndex,
                                        LocalType = locException.LocalType
                                    },
                                    GetStoredObject(mappingItemId, typeof(IMappingOperation))
                                }
                        },
                };


            /* Unmerged change from project 'EmitMapper (netstandard2.1)'
            Before:
                        var tryCatch = new AstExceptionHandlingBlock(
            After:
                        var tryCatch = new AstExceptionHandlingBlock(
            */
            AstExceptionHandlingBlock tryCatch = new AstExceptionHandlingBlock(
                writeValue,
                handler,
                typeof(Exception),
                locException
                );
            return tryCatch;
        }

        private IAstNode WriteMappingValue(
            IMappingOperation mappingOperation,
            int mappingItemId,
            IAstRefOrValue value)
        {
            IAstNode writeValue;

            if (mappingOperation is SrcReadOperation)
            {
                writeValue = AstBuildHelper.CallMethod(
                    typeof(ValueSetter).GetMethod("Invoke"),
                    new AstCastclassRef(
                        (IAstRef)AstBuildHelper.ReadMemberRV(
                             GetStoredObject(mappingItemId, typeof(SrcReadOperation)),
                             typeof(SrcReadOperation).GetProperty("Setter")
                         ),
                         (mappingOperation as SrcReadOperation).Setter.GetType()
                    ),
                    new List<IAstStackItem>()
                        {
                            AstBuildHelper.ReadLocalRV(locTo),
                            value,
                            AstBuildHelper.ReadLocalRV(locState),
                        }
                    );
            }
            else
            {
                writeValue = AstBuildHelper.WriteMembersChain(
                    (mappingOperation as IDestOperation).Destination.MembersChain,
                    AstBuildHelper.ReadLocalRA(locTo),
                    value
                );
            }
            return writeValue;
        }

        private IAstRefOrValue ConvertMappingValue(
            ReadWriteSimple rwMapOp,
            int operationId,
            IAstRefOrValue sourceValue)
        {
            IAstRefOrValue convertedValue = sourceValue;
            if (rwMapOp.Converter != null)
            {
                convertedValue = AstBuildHelper.CallMethod(
                    rwMapOp.Converter.GetType().GetMethod("Invoke"),
                    new AstCastclassRef(
                        (IAstRef)AstBuildHelper.ReadMemberRV(
                             GetStoredObject(operationId, typeof(ReadWriteSimple)),
                             typeof(ReadWriteSimple).GetProperty("Converter")
                        ),
                        rwMapOp.Converter.GetType()
                    ),
                    new List<IAstStackItem>()
                    {
                        sourceValue,
                        AstBuildHelper.ReadLocalRV(locState),
                    }
                );
            }
            else
            {
                if (rwMapOp.ShallowCopy && rwMapOp.Destination.MemberType == rwMapOp.Source.MemberType)
                {
                    convertedValue = sourceValue;
                }
                else
                {

                    /* Unmerged change from project 'EmitMapper (netstandard2.1)'
                    Before:
                                        var mi = staticConvertersManager.GetStaticConverter(rwMapOp.Source.MemberType, rwMapOp.Destination.MemberType);
                    After:
                                        var mi = staticConvertersManager.GetStaticConverter(rwMapOp.Source.MemberType, rwMapOp.Destination.MemberType);
                    */
                    MethodInfo mi = staticConvertersManager.GetStaticConverter(rwMapOp.Source.MemberType, rwMapOp.Destination.MemberType);
                    if (mi != null)
                    {
                        convertedValue = AstBuildHelper.CallMethod(
                            mi,
                            null,
                            new List<IAstStackItem> { sourceValue }
                        );
                    }
                    else
                    {
                        convertedValue = ConvertByMapper(rwMapOp);
                    }
                }
            }

            return convertedValue;
        }

        private IAstRefOrValue ConvertByMapper(ReadWriteSimple mapping)
        {
            IAstRefOrValue convertedValue;
            ObjectsMapperDescr mapper = objectsMapperManager.GetMapperInt(
                mapping.Source.MemberType,
                mapping.Destination.MemberType,
                mappingConfigurator);
            int mapperId = AddObjectToStore(mapper);

            convertedValue = AstBuildHelper.CallMethod(
                typeof(ObjectsMapperBaseImpl).GetMethod(
                    "Map",
                    new Type[] { typeof(object), typeof(object), typeof(object) }
                    ),

                new AstReadFieldRef
                {
                    FieldInfo = typeof(ObjectsMapperDescr).GetField("mapper"),
                    SourceObject = GetStoredObject(mapperId, mapper.GetType())
                },

                new List<IAstStackItem>()
                    {
                        AstBuildHelper.ReadMembersChain(
                            AstBuildHelper.ReadLocalRA(locFrom),
                            mapping.Source.MembersChain
                        ),
                        AstBuildHelper.ReadMembersChain(
                            AstBuildHelper.ReadLocalRA(locTo),
                            mapping.Destination.MembersChain
                        ),
                        (IAstRef)AstBuildHelper.ReadLocalRA(locState)
                    }
                );
            return convertedValue;
        }

        private IAstNode ProcessDestSrcReadOperation(
            DestSrcReadOperation operation,
            int operationId)
        {
            IAstRefOrValue src =
                AstBuildHelper.ReadMembersChain(
                    AstBuildHelper.ReadLocalRA(locFrom),
                    operation.Source.MembersChain
                );

            IAstRefOrValue dst =
                AstBuildHelper.ReadMembersChain(
                    AstBuildHelper.ReadLocalRA(locFrom),
                    operation.Destination.MembersChain
                );

            return AstBuildHelper.CallMethod(
                typeof(ValueProcessor).GetMethod("Invoke"),
                new AstCastclassRef(
                    (IAstRef)AstBuildHelper.ReadMemberRV(
                        GetStoredObject(operationId, typeof(DestSrcReadOperation)),
                        typeof(DestWriteOperation).GetProperty("ValueProcessor")
                    ),
                    operation.ValueProcessor.GetType()
                ),
                new List<IAstStackItem> { src, dst, AstBuildHelper.ReadLocalRV(locState) }
            );
        }

        private IAstRefOrValue ReadSrcMappingValue(
            IMappingOperation mapping,
            int operationId)
        {

            /* Unmerged change from project 'EmitMapper (netstandard2.1)'
            Before:
                        var readOp = mapping as ISrcReadOperation;
            After:
                        var readOp = mapping as ISrcReadOperation;
            */
            ISrcReadOperation readOp = mapping as ISrcReadOperation;
            if (readOp != null)
            {
                return AstBuildHelper.ReadMembersChain(
                    AstBuildHelper.ReadLocalRA(locFrom),
                    readOp.Source.MembersChain
                );
            }


            /* Unmerged change from project 'EmitMapper (netstandard2.1)'
            Before:
                        var destWriteOp = (DestWriteOperation)mapping;
            After:
                        var destWriteOp = (DestWriteOperation)mapping;
            */
            DestWriteOperation destWriteOp = (DestWriteOperation)mapping;
            if (destWriteOp.Getter != null)
            {
                return AstBuildHelper.CallMethod(
                    destWriteOp.Getter.GetType().GetMethod("Invoke"),
                    new AstCastclassRef(
                        (IAstRef)AstBuildHelper.ReadMemberRV(
                            GetStoredObject(operationId, typeof(DestWriteOperation)),
                            typeof(DestWriteOperation).GetProperty("Getter")
                        ),
                        destWriteOp.Getter.GetType()
                    ),
                    new List<IAstStackItem>
                    {
                        AstBuildHelper.ReadLocalRV(locState)
                    }
                );
            }
            throw new EmitMapperException("Invalid mapping operations");
        }

        private static IAstRef GetStoredObject(int objectIndex, Type castType)
        {

            /* Unmerged change from project 'EmitMapper (netstandard2.1)'
            Before:
                        var result = (IAstRef)AstBuildHelper.ReadArrayItemRV(
            After:
                        var result = (IAstRef)AstBuildHelper.ReadArrayItemRV(
            */
            IAstRef result = (IAstRef)AstBuildHelper.ReadArrayItemRV(
                (IAstRef)AstBuildHelper.ReadFieldRA(
                    new AstReadThis() { ThisType = typeof(ObjectsMapperBaseImpl) },
                    typeof(ObjectsMapperBaseImpl).GetField(
                        "StroredObjects",
                        BindingFlags.Instance | BindingFlags.Public
                    )
                ),
                objectIndex
            );
            if (castType != null)
            {
                result = new AstCastclassRef(result, castType);
            }
            return result;
        }
    }
}
