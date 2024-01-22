namespace EmitMapper.EmitBuilders;

/// <summary>
///   The mapping operations processor.
/// </summary>
internal class MappingOperationsProcessor
{
	private static readonly MethodInfo MapMethod = Metadata<MapperBase>.Type.GetMethod(
	  nameof(MapperBase.Map),
	  new[] { Metadata<object>.Type, Metadata<object>.Type, Metadata<object>.Type });

	private static PropertyInfo hasValue = Metadata.Nullable1.GetProperty("HasValue");

	/// <summary>
	/// Compilation context
	/// </summary>
	public CompilationContext CompilationContext;

	/// <summary>
	/// Loc exception
	/// </summary>
	public LocalBuilder LocException;

	/// <summary>
	/// Loc from
	/// </summary>
	public LocalBuilder LocFrom;

	/// <summary>
	/// Loc state
	/// </summary>
	public LocalBuilder LocState;

	/// <summary>
	/// Loc to
	/// </summary>
	public LocalBuilder LocTo;

	/// <summary>
	/// Mapping configurator
	/// </summary>
	public IMappingConfigurator? MappingConfigurator;

	/// <summary>
	/// Objects mapper manager
	/// </summary>
	public Mapper ObjectsMapperManager;

	/// <summary>
	/// Operations
	/// </summary>
	public IEnumerable<IMappingOperation> Operations = new List<IMappingOperation>();

	/// <summary>
	/// Root operation
	/// </summary>
	public IRootMappingOperation RootOperation;

	/// <summary>
	/// Static converters manager
	/// </summary>
	public StaticConvertersManager StaticConvertersManager;

	/// <summary>
	/// Stored objects
	/// </summary>
	public List<object> StoredObjects = new();

	/// <summary>
	///   Initializes a new instance of the <see cref="MappingOperationsProcessor" /> class.
	/// </summary>
	public MappingOperationsProcessor()
	{
	}

	/// <summary>
	///   Initializes a new instance of the <see cref="MappingOperationsProcessor" /> class.
	/// </summary>
	/// <param name="prototype">The prototype.</param>
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

	/// <summary>
	///   Processes the operations.
	/// </summary>
	/// <returns>An IAstNode.</returns>
	public IAstNode ProcessOperations()
	{
		var result = new AstComplexNode();

		foreach (var operation in Operations)
		{
			var operationId = AddObjectToStore(operation);

			var completeOperation = operation switch
			{
				OperationsBlock block => new MappingOperationsProcessor(this) { Operations = block.Operations }
				  .ProcessOperations(),
				ReadWriteComplex complex => Process_ReadWriteComplex(complex, operationId),
				DestSrcReadOperation readOperation => ProcessDestSrcReadOperation(readOperation, operationId),
				SrcReadOperation srcReadOperation => ProcessSrcReadOperation(srcReadOperation, operationId),
				DestWriteOperation writeOperation => ProcessDestWriteOperation(writeOperation, operationId),
				ReadWriteSimple simple => ProcessReadWriteSimple(simple, operationId),
				_ => null
			};

			switch (completeOperation)
			{
				case null:
					continue;
			}

			if (LocException is not null)
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

	/// <summary>
	///   Gets the stored object.
	/// </summary>
	/// <param name="objectIndex">The object index.</param>
	/// <param name="castType">The cast type.</param>
	/// <returns>An IAstRef.</returns>
	private static IAstRef GetStoredObject(int objectIndex, Type? castType)
	{
		var result = (IAstRef)AstBuildHelper.IReadArrayItemRv(
		  (IAstRef)AstBuildHelper.IReadFieldRa(
			new AstReadThis { ThisType = Metadata<MapperBase>.Type },
			Metadata<MapperBase>.Type.GetField(
			  nameof(MapperBase.StoredObjects),
			  BindingFlags.Instance | BindingFlags.Public)),
		  objectIndex);

		if (castType is not null)
		{
			result = new AstCastclassRef(result, castType);
		}

		return result;
	}

	/// <summary>
	///   Adds the object to store.
	/// </summary>
	/// <param name="obj">The obj.</param>
	/// <returns>An int.</returns>
	private int AddObjectToStore(object obj)
	{
		var objectId = StoredObjects.Count;
		StoredObjects.Add(obj);

		return objectId;
	}

	/// <summary>
	///   Converts the by mapper.
	/// </summary>
	/// <param name="mapping">The mapping.</param>
	/// <returns>An IAstRefOrValue.</returns>
	private IAstRefOrValue ConvertByMapper(ReadWriteSimple mapping)
	{
		var mapper = ObjectsMapperManager.GetMapperDescription(
		  mapping.Source.MemberType,
		  mapping.Destination.MemberType,
		  MappingConfigurator);

		var mapperId = AddObjectToStore(mapper);

		var convertedValue = AstBuildHelper.ICallMethod(
		  MapMethod,
		  new AstReadFieldRef
		  {
			  FieldInfo = Metadata<MapperDescription>.Type.GetField(nameof(MapperDescription.Mapper)),
			  SourceObject = GetStoredObject(mapperId, mapper.GetType())
		  },
		  new List<IAstStackItem>
		  {
		AstBuildHelper.IReadMembersChain(AstBuildHelper.IReadLocalRa(LocFrom), mapping.Source.MembersChain),
		AstBuildHelper.IReadMembersChain(AstBuildHelper.IReadLocalRa(LocTo), mapping.Destination.MembersChain),
		(IAstRef)AstBuildHelper.IReadLocalRa(LocState)
		  });

		return convertedValue;
	}

	/// <summary>
	///   Converts the mapping value.
	/// </summary>
	/// <param name="rwMapOp">The rw map op.</param>
	/// <param name="operationId">The operation id.</param>
	/// <param name="sourceValue">The source value.</param>
	/// <returns>An IAstRefOrValue.</returns>
	private IAstRefOrValue ConvertMappingValue(ReadWriteSimple rwMapOp, int operationId, IAstRefOrValue sourceValue)
	{
		IAstRefOrValue convertedValue;

		if (rwMapOp.Converter is not null)
		{
			var t = rwMapOp.Converter.GetType();

			convertedValue = AstBuildHelper.ICallMethod(
			  t.GetMethodCache("Invoke"),
			  new AstCastclassRef(
				(IAstRef)AstBuildHelper.IReadMemberRv(
				  GetStoredObject(operationId, Metadata<ReadWriteSimple>.Type),
				  Metadata<ReadWriteSimple>.Type.GetProperty(nameof(ReadWriteSimple.Converter))),
				t), [sourceValue, AstBuildHelper.IReadLocalRv(LocState)]);
		}
		else
		{
			switch (rwMapOp.ShallowCopy)
			{
				case true when rwMapOp.Destination.MemberType == rwMapOp.Source.MemberType:
					convertedValue = sourceValue;

					break;
				default:
				{
					var mi = StaticConvertersManager.GetStaticConverter(rwMapOp.Source.MemberType, rwMapOp.Destination.MemberType);

					if (mi is not null)
					{
						convertedValue = AstBuildHelper.ICallMethod(mi, null, [sourceValue]);
					}
					else
					{
						convertedValue = ConvertByMapper(rwMapOp);
					}

					break;
				}
			}
		}

		return convertedValue;
	}

	/// <summary>
	///   Creates the exception handling block.
	/// </summary>
	/// <param name="mappingItemId">The mapping item id.</param>
	/// <param name="writeValue">The write value.</param>
	/// <returns>An IAstNode.</returns>
	private IAstNode CreateExceptionHandlingBlock(int mappingItemId, IAstNode writeValue)
	{
		var handler = new AstThrow
		{
			Exception = new AstNewObject
			{
				ObjectType = Metadata<EmitMapperException>.Type,
				ConstructorParams = new IAstStackItem[]
			{
		  new AstConstantString { Str = "Error in mapping operation execution: " },
		  new AstReadLocalRef { LocalIndex = LocException.LocalIndex, LocalType = LocException.LocalType },
		  GetStoredObject(
			mappingItemId,
			Metadata<IMappingOperation>.Type)
			}
			}
		};

		var tryCatch = new AstExceptionHandlingBlock(writeValue, handler, Metadata<Exception>.Type, LocException);

		return tryCatch;
	}

	/// <summary>
	///   Gets the null value.
	/// </summary>
	/// <param name="nullSubstitutor">The null substitutor.</param>
	/// <returns>An IAstRefOrValue.</returns>
	private IAstRefOrValue GetNullValue(Delegate? nullSubstitutor)
	{
		if (nullSubstitutor is not null)
		{
			var t = nullSubstitutor.GetType();
			var substId = AddObjectToStore(nullSubstitutor);

			return AstBuildHelper.ICallMethod(
			  t.GetMethodCache("Invoke"),
			  GetStoredObject(substId, t),
			  new List<IAstStackItem> { AstBuildHelper.IReadLocalRv(LocState) });
		}

		return new AstConstantNull();
	}

	/// <summary>
	///   Process_s the destination filter.
	/// </summary>
	/// <param name="op">The op.</param>
	/// <param name="operationId">The operation id.</param>
	/// <param name="result">The result.</param>
	/// <returns>An IAstNode.</returns>
	private IAstNode Process_DestinationFilter(IReadWriteOperation op, int operationId, IAstNode result)
	{
		return Process_ValuesFilter(
		  op,
		  operationId,
		  result,
		  AstBuildHelper.IReadMembersChain(AstBuildHelper.IReadLocalRa(LocTo), op.Destination.MembersChain),
		  nameof(IReadWriteOperation.DestinationFilter),
		  op.DestinationFilter);
	}

	/// <summary>
	///   Process_s the read write complex.
	/// </summary>
	/// <param name="op">The op.</param>
	/// <param name="operationId">The operation id.</param>
	/// <returns>An IAstNode.</returns>
	private IAstNode Process_ReadWriteComplex(ReadWriteComplex op, int operationId)
	{
		var result = op.Converter is not null
		  ? Process_ReadWriteComplex_ByConverter(op, operationId)
		  : Process_ReadWriteComplex_Copying(op);

		if (op.SourceFilter is not null)
		{
			result = Process_SourceFilter(op, operationId, result);
		}

		if (op.DestinationFilter is not null)
		{
			result = Process_DestinationFilter(op, operationId, result);
		}

		return result;
	}

	/// <summary>
	///   Process_s the read write complex_ by converter.
	/// </summary>
	/// <param name="op">The op.</param>
	/// <param name="operationId">The operation id.</param>
	/// <returns>An IAstNode.</returns>
	private IAstNode Process_ReadWriteComplex_ByConverter(ReadWriteComplex op, int operationId)
	{
		var t = op.Converter?.GetType();

		var result = AstBuildHelper.IWriteMembersChain(
		  op.Destination?.MembersChain,
		  AstBuildHelper.IReadLocalRa(LocTo),
		  AstBuildHelper.ICallMethod(
			t.GetMethodCache("Invoke"),
			new AstCastclassRef(
			  (IAstRef)AstBuildHelper.IReadMemberRv(
				GetStoredObject(operationId, Metadata<ReadWriteComplex>.Type),
				Metadata<ReadWriteComplex>.Type.GetProperty(nameof(ReadWriteComplex.Converter))),
			  t), [
				  AstBuildHelper.IReadMembersChain(AstBuildHelper.IReadLocalRa(LocFrom), op.Source.MembersChain),
				  AstBuildHelper.IReadLocalRv(LocState)
				  ]));

		return result;
	}

	/// <summary>
	///   Process_s the read write complex_ copying.
	/// </summary>
	/// <param name="op">The op.</param>
	/// <returns>An IAstNode.</returns>
	private IAstNode Process_ReadWriteComplex_Copying(ReadWriteComplex op)
	{
		var result = new AstComplexNode();
		var tempSrc = CompilationContext.IlGenerator.DeclareLocal(op.Source.MemberType);
		var tempDst = CompilationContext.IlGenerator.DeclareLocal(op.Destination.MemberType);
		var origTempSrc = tempSrc;
		var origTempDst = tempDst;

		result.Nodes.Add(
		  new AstWriteLocal(
			tempSrc,
			AstBuildHelper.IReadMembersChain(AstBuildHelper.IReadLocalRa(LocFrom), op.Source.MembersChain)));

		result.Nodes.Add(
		  new AstWriteLocal(
			tempDst,
			AstBuildHelper.IReadMembersChain(AstBuildHelper.IReadLocalRa(LocTo), op.Destination.MembersChain)));

		var writeNullToDest = new List<IAstNode>
	{
	  AstBuildHelper.IWriteMembersChain(
		op.Destination.MembersChain,
		AstBuildHelper.IReadLocalRa(LocTo),
		GetNullValue(op.NullSubstitutor))
	};

		// Target construction
		var initDest = new List<IAstNode>();
		var custCtr = op.TargetConstructor;

		if (custCtr is not null)
		{
			var custCtrIdx = AddObjectToStore(custCtr);
			var t = custCtr.GetType();

			initDest.Add(
			  new AstWriteLocal(
				tempDst,
				AstBuildHelper.ICallMethod(t.GetMethodCache("Invoke"), GetStoredObject(custCtrIdx, t), null)));
		}
		else
		{
			initDest.Add(new AstWriteLocal(tempDst, new AstNewObject(op.Destination.MemberType, null)));
		}

		var copying = new List<IAstNode>();

		// if destination is nullable, create a temp target variable with underlying destination type
		if (ReflectionHelper.IsNullable(op.Source.MemberType))
		{
			tempSrc = CompilationContext.IlGenerator.DeclareLocal(op.Source.MemberType.GetUnderlyingTypeCache());

			copying.Add(
			  new AstWriteLocal(
				tempSrc,
				AstBuildHelper.IReadPropertyRv(
				  AstBuildHelper.IReadLocalRa(origTempSrc),
				  op.Source.MemberType.GetProperty("Value"))));
		}

		// If destination is null, initialize it.
		if (ReflectionHelper.IsNullable(op.Destination.MemberType) || !op.Destination.MemberType.IsValueType)
		{
			copying.Add(
			  new AstIf
			  {
				  Condition = ReflectionHelper.IsNullable(op.Destination.MemberType)
				  ? new AstExprNot(
					(IAstValue)AstBuildHelper.IReadPropertyRv(
					  AstBuildHelper.IReadLocalRa(origTempDst),
					  op.Destination.MemberType.GetProperty("HasValue")))
				  : new AstExprIsNull(AstBuildHelper.IReadLocalRv(origTempDst)),
				  TrueBranch = new AstComplexNode { Nodes = initDest }
			  });

			if (ReflectionHelper.IsNullable(op.Destination.MemberType))
			{
				tempDst = CompilationContext.IlGenerator.DeclareLocal(op.Destination.MemberType.GetUnderlyingTypeCache());

				copying.Add(
				  new AstWriteLocal(
					tempDst,
					AstBuildHelper.IReadPropertyRv(
					  AstBuildHelper.IReadLocalRa(origTempDst),
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
			RootOperation = MappingConfigurator.GetRootMappingOperation(
			  op.Source.MemberType,
			  op.Destination.MemberType)
		  }.ProcessOperations()
			}
		  });

		IAstRefOrValue processedValue;

		if (ReflectionHelper.IsNullable(op.Destination.MemberType))
			processedValue = new AstNewObject(
			  op.Destination.MemberType,
			  new IAstStackItem[] { AstBuildHelper.IReadLocalRv(tempDst) });
		else
		{
			processedValue = AstBuildHelper.IReadLocalRv(origTempDst);
		}

		if (op.ValuesPostProcessor is not null)
		{
			var t = op.ValuesPostProcessor.GetType();
			var postProcessorId = AddObjectToStore(op.ValuesPostProcessor);

			processedValue = AstBuildHelper.ICallMethod(
			  t.GetMethodCache("Invoke"),
			  GetStoredObject(postProcessorId, t),
			  new List<IAstStackItem> { processedValue, AstBuildHelper.IReadLocalRv(LocState) });
		}

		copying.Add(
		  AstBuildHelper.IWriteMembersChain(op.Destination.MembersChain, AstBuildHelper.IReadLocalRa(LocTo), processedValue));

		if (ReflectionHelper.IsNullable(op.Source.MemberType) || !op.Source.MemberType.IsValueType)
			result.Nodes.Add(
			  new AstIf
			  {
				  Condition = ReflectionHelper.IsNullable(op.Source.MemberType)
				  ? new AstExprNot(
					(IAstValue)AstBuildHelper.IReadPropertyRv(
					  AstBuildHelper.IReadLocalRa(origTempSrc),
					  op.Source.MemberType.GetProperty("HasValue")))
				  : new AstExprIsNull(AstBuildHelper.IReadLocalRv(origTempSrc)),
				  TrueBranch = new AstComplexNode { Nodes = writeNullToDest },
				  FalseBranch = new AstComplexNode { Nodes = copying }
			  });
		else
		{
			result.Nodes.AddRange(copying);
		}

		return result;
	}

	/// <summary>
	///   Process_s the source filter.
	/// </summary>
	/// <param name="op">The op.</param>
	/// <param name="operationId">The operation id.</param>
	/// <param name="result">The result.</param>
	/// <returns>An IAstNode.</returns>
	private IAstNode Process_SourceFilter(IReadWriteOperation op, int operationId, IAstNode result)
	{
		return Process_ValuesFilter(
		  op,
		  operationId,
		  result,
		  AstBuildHelper.IReadMembersChain(AstBuildHelper.IReadLocalRa(LocFrom), op.Source.MembersChain),
		  nameof(IReadWriteOperation.SourceFilter),
		  op.SourceFilter);
	}

	/// <summary>
	///   Process_s the values filter.
	/// </summary>
	/// <param name="op">The op.</param>
	/// <param name="operationId">The operation id.</param>
	/// <param name="result">The result.</param>
	/// <param name="value">The value.</param>
	/// <param name="fieldName">The field name.</param>
	/// <param name="filterDelegate">The filter delegate.</param>
	/// <returns>An IAstNode.</returns>
	private IAstNode Process_ValuesFilter(
	  IReadWriteOperation op,
	  int operationId,
	  IAstNode result,
	  IAstRefOrValue value,
	  string fieldName,
	  Delegate filterDelegate)
	{
		var delegateType = filterDelegate.GetType();

		result = new AstComplexNode
		{
			Nodes = new List<IAstNode>
	  {
		new AstIf
		{
		  Condition = (IAstValue)AstBuildHelper.ICallMethod(
			delegateType.GetMethodCache("Invoke"),
			new AstCastclassRef(
			  (IAstRef)AstBuildHelper.IReadMemberRv(
				GetStoredObject(operationId, Metadata<IReadWriteOperation>.Type),
				Metadata<IReadWriteOperation>.Type.GetProperty(fieldName)),
			  delegateType),
			new List<IAstStackItem> { value, AstBuildHelper.IReadLocalRv(LocState) }),
		  TrueBranch = new AstComplexNode { Nodes = new List<IAstNode> { result } }
		}
	  }
		};

		return result;
	}

	/// <summary>
	///   Processes the dest src read operation.
	/// </summary>
	/// <param name="operation">The operation.</param>
	/// <param name="operationId">The operation id.</param>
	/// <returns>An IAstNode.</returns>
	private IAstNode ProcessDestSrcReadOperation(DestSrcReadOperation operation, int operationId)
	{
		var src = AstBuildHelper.IReadMembersChain(AstBuildHelper.IReadLocalRa(LocFrom), operation.Source.MembersChain);

		var dst = AstBuildHelper.IReadMembersChain(AstBuildHelper.IReadLocalRa(LocFrom), operation.Destination.MembersChain);

		return AstBuildHelper.ICallMethod(
		  Metadata<ValueProcessor>.Type.GetMethodCache(nameof(ValueProcessor.Invoke)),
		  new AstCastclassRef(
			(IAstRef)AstBuildHelper.IReadMemberRv(
			  GetStoredObject(operationId, Metadata<DestSrcReadOperation>.Type),
			  Metadata<DestSrcReadOperation>.Type.GetProperty(nameof(DestSrcReadOperation.ValueProcessor))),
			operation.ValueProcessor.GetType()),
		  new List<IAstStackItem> { src, dst, AstBuildHelper.IReadLocalRv(LocState) });
	}

	/// <summary>
	///   Processes the dest write operation.
	/// </summary>
	/// <param name="destWriteOperation">The dest write operation.</param>
	/// <param name="operationId">The operation id.</param>
	/// <returns>An IAstNode.</returns>
	private IAstNode ProcessDestWriteOperation(DestWriteOperation destWriteOperation, int operationId)
	{
		var locValueToWrite = CompilationContext.IlGenerator.DeclareLocal(destWriteOperation.Getter.Method.ReturnType);
		var t = destWriteOperation.Getter.GetType();

		var cmdValue = new AstWriteLocal(
		  locValueToWrite,
		  AstBuildHelper.ICallMethod(
			t.GetMethodCache("Invoke"),
			new AstCastclassRef(
			  (IAstRef)AstBuildHelper.IReadMemberRv(
				GetStoredObject(operationId, Metadata<DestWriteOperation>.Type),
				Metadata<DestWriteOperation>.Type.GetProperty(nameof(DestWriteOperation.Getter))),
			  t),
			new List<IAstStackItem> { AstBuildHelper.IReadLocalRv(LocFrom), AstBuildHelper.IReadLocalRv(LocState) }));

		// todo: need to add unit test for this method
		return new AstComplexNode
		{
			Nodes = new List<IAstNode>
	  {
		cmdValue,
		new AstIf
		{
		  Condition = new AstExprEquals(
			(IAstValue)AstBuildHelper.IReadMembersChain(
			  AstBuildHelper.IReadLocalRa(locValueToWrite),
			  locValueToWrite.LocalType.GetField(nameof(ValueToWrite<object>.Action))),
			new AstConstantInt32 { Value = 0 }),
		  TrueBranch = new AstComplexNode
		  {
			Nodes = new List<IAstNode>
			{
			  AstBuildHelper.IWriteMembersChain(
				destWriteOperation.Destination.MembersChain,
				AstBuildHelper.IReadLocalRa(LocTo),
				AstBuildHelper.IReadMembersChain(
				  AstBuildHelper.IReadLocalRa(locValueToWrite),
				  locValueToWrite.LocalType.GetField(
					nameof(ValueToWrite<object>.Value))))
			}
		  }
		}
	  }
		};
	}

	/// <summary>
	///   Processes the read write simple.
	/// </summary>
	/// <param name="readWriteSimple">The read write simple.</param>
	/// <param name="operationId">The operation id.</param>
	/// <returns>An IAstNode.</returns>
	private IAstNode ProcessReadWriteSimple(ReadWriteSimple readWriteSimple, int operationId)
	{
		var sourceValue = ReadSrcMappingValue(readWriteSimple, operationId);

		IAstRefOrValue convertedValue;

		if (readWriteSimple.NullSubstitutor is not null && (ReflectionHelper.IsNullable(readWriteSimple.Source.MemberType)
														|| !readWriteSimple.Source.MemberType.IsValueType))
			convertedValue = new AstIfTernar(
			  ReflectionHelper.IsNullable(readWriteSimple.Source.MemberType)
				? new AstExprNot(
				  AstBuildHelper.IReadPropertyRv(
					new AstValueToAddr((IAstValue)sourceValue),
					readWriteSimple.Source.MemberType.GetProperty("HasValue")))
				: new AstExprIsNull(sourceValue),
			  GetNullValue(readWriteSimple.NullSubstitutor), // source is null
			  AstBuildHelper.ICastClass(
				ConvertMappingValue(readWriteSimple, operationId, sourceValue),
				readWriteSimple.Destination.MemberType));
		else
		{
			convertedValue = ConvertMappingValue(readWriteSimple, operationId, sourceValue);
		}

		var result = WriteMappingValue(readWriteSimple, operationId, convertedValue);

		if (readWriteSimple.SourceFilter is not null)
		{
			result = Process_SourceFilter(readWriteSimple, operationId, result);
		}

		if (readWriteSimple.DestinationFilter is not null)
		{
			result = Process_DestinationFilter(readWriteSimple, operationId, result);
		}

		return result;
	}

	/// <summary>
	///   Processes the src read operation.
	/// </summary>
	/// <param name="srcReadOperation">The src read operation.</param>
	/// <param name="operationId">The operation id.</param>
	/// <returns>An IAstNode.</returns>
	private IAstNode ProcessSrcReadOperation(SrcReadOperation srcReadOperation, int operationId)
	{
		var value = AstBuildHelper.IReadMembersChain(
		  AstBuildHelper.IReadLocalRa(LocFrom),
		  srcReadOperation.Source.MembersChain);

		return WriteMappingValue(srcReadOperation, operationId, value);
	}

	/// <summary>
	///   Reads the src mapping value.
	/// </summary>
	/// <param name="mapping">The mapping.</param>
	/// <param name="operationId">The operation id.</param>
	/// <exception cref="EmitMapperException"></exception>
	/// <returns>An IAstRefOrValue.</returns>
	private IAstRefOrValue ReadSrcMappingValue(IMappingOperation mapping, int operationId)
	{
		switch (mapping)
		{
			case ISrcReadOperation readOp:
				return AstBuildHelper.IReadMembersChain(AstBuildHelper.IReadLocalRa(LocFrom), readOp.Source.MembersChain);
		}

		var destWriteOp = (DestWriteOperation)mapping;

		if (destWriteOp.Getter is not null)
		{
			var t = destWriteOp.Getter.GetType();

			return AstBuildHelper.ICallMethod(
			  t.GetMethodCache("Invoke"),
			  new AstCastclassRef(
				(IAstRef)AstBuildHelper.IReadMemberRv(
				  GetStoredObject(operationId, Metadata<DestWriteOperation>.Type),
				  Metadata<DestWriteOperation>.Type.GetProperty(nameof(DestWriteOperation.Getter))),
				t),
			  new List<IAstStackItem> { AstBuildHelper.IReadLocalRv(LocState) });
		}

		throw new EmitMapperException("Invalid mapping operations");
	}

	/// <summary>
	///   Writes the mapping value.
	/// </summary>
	/// <param name="mappingOperation">The mapping operation.</param>
	/// <param name="mappingItemId">The mapping item id.</param>
	/// <param name="value">The value.</param>
	/// <returns>An IAstNode.</returns>
	private IAstNode WriteMappingValue(IMappingOperation mappingOperation, int mappingItemId, IAstRefOrValue value)
	{
		IAstNode writeValue;

		switch (mappingOperation)
		{
			case SrcReadOperation srcReadOperation:
				writeValue = AstBuildHelper.ICallMethod(
					Metadata<ValueSetter>.Type.GetMethodCache(nameof(ValueSetter.Invoke)),
					new AstCastclassRef(
						(IAstRef)AstBuildHelper.IReadMemberRv(
							GetStoredObject(mappingItemId, Metadata<SrcReadOperation>.Type),
							Metadata<SrcReadOperation>.Type.GetProperty(nameof(SrcReadOperation.Setter))),
						srcReadOperation.Setter.GetType()),
					new List<IAstStackItem> { AstBuildHelper.IReadLocalRv(LocTo), value, AstBuildHelper.IReadLocalRv(LocState) });

				break;
			default:
				writeValue = AstBuildHelper.IWriteMembersChain(
					(mappingOperation as IDestOperation)?.Destination.MembersChain,
					AstBuildHelper.IReadLocalRa(LocTo),
					value);

				break;
		}

		return writeValue;
	}
}