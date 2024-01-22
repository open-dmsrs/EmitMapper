using EmitMapper.EmitInvoker.Delegates;

namespace EmitMapper.Mappers;

/// <summary>
///   Base class for Mappers
/// </summary>
public abstract class MapperBase
{
	/// <summary>
	/// Stored objects
	/// </summary>
	public object[]? StoredObjects;

	/// <summary>
	///   Mapper manager
	/// </summary>
	internal Mapper? Mapper;

	/// <summary>
	///   True, if reference properties and members of same type should
	///   be copied by reference (shallow copy, without creating new instance for destination object)
	/// </summary>
	internal bool ShallowCopy = true;

	/// <summary>
	///   Type of source object
	/// </summary>
	internal Type TypeFrom;

	/// <summary>
	///   Type of destination object
	/// </summary>
	internal Type TypeTo;

	protected DelegateInvokerFunc2? Converter;

	protected DelegateInvokerFunc2? DestinationFilter;

	protected IMappingConfigurator? MappingConfigurator;

	protected DelegateInvokerFunc0? NullSubstitutor;

	protected IRootMappingOperation? RootOperation;

	protected DelegateInvokerFunc2? SourceFilter;

	protected DelegateInvokerFunc0? TargetConstructor;

	protected DelegateInvokerFunc2? ValuesPostProcessor;

	/// <summary>
	///   Creates an instance of destination object
	/// </summary>
	/// <returns>Destination object</returns>
	public abstract object? CreateTargetInstance();

	// public IMappingConfigurator MappingConfigurator => this._mappingConfigurator;

	/// <summary>
	///   Copies object properties and members of "from" to object "to"
	/// </summary>
	/// <param name="from">Source object</param>
	/// <param name="to">Destination object</param>
	/// <param name="state">state</param>
	public virtual object? Map(object? from, object? to, object? state)
	{
		object? result;
		var filter = SourceFilter?.CallFunc(from, state);
		if (filter is not null && !(bool)filter)
		{
			return to;
		}

		filter = DestinationFilter?.CallFunc(to, state);
		if (filter is not null && !(bool)filter)
		{
			return to;
		}

		switch (from)
		{
			case null:
				result = NullSubstitutor?.CallFunc();

				break;
			default:
			{
				if (Converter is not null)
				{
					result = Converter.CallFunc(from, state);
				}
				else
				{
					to ??= ConstructTarget();

					result = MapCore(from, to, state);
				}

				break;
			}
		}

		if (ValuesPostProcessor is not null)
		{
			result = ValuesPostProcessor.CallFunc(result, state);
		}

		return result;
	}

	/// <summary>
	///   Creates new instance of destination object and initializes it by values from "from" object
	/// </summary>
	/// <param name="from">source object</param>
	public virtual object? Map(object? from)
	{
		switch (from)
		{
			case null:
				return null;
			default:
				return Map(from, ConstructTarget(), null);
		}
	}

	/// <summary>
	///   Copies object properties and members of "from" to object "to"
	/// </summary>
	/// <param name="from">Source object</param>
	/// <param name="to">Destination object</param>
	/// <param name="state">state</param>
	public abstract object? MapCore(object? from, object? to, object? state);

	/// <summary>
	/// </summary>
	/// <param name="objectMapperManager">The object mapper manager.</param>
	/// <param name="typeFrom">The type from.</param>
	/// <param name="typeTo">The type to.</param>
	/// <param name="mappingConfigurator">The mapping configurator.</param>
	/// <param name="storedObjects">The stored objects.</param>
	internal void Initialize(
	  Mapper? objectMapperManager,
	  Type typeFrom,
	  Type typeTo,
	  IMappingConfigurator? mappingConfigurator,
	  object[] storedObjects)
	{
		Mapper = objectMapperManager;
		TypeFrom = typeFrom;
		TypeTo = typeTo;
		this.MappingConfigurator = mappingConfigurator;
		StoredObjects = storedObjects;

		if (this.MappingConfigurator is not null)
		{
			RootOperation = this.MappingConfigurator.GetRootMappingOperation(typeFrom, typeTo)
							?? new RootMappingOperation(typeFrom, typeTo);

			var constructor = RootOperation.TargetConstructor;

			if (constructor is not null)
			{
				TargetConstructor = (DelegateInvokerFunc0)DelegateInvoker.GetDelegateInvoker(constructor);
			}

			var rootOperationValuesPostProcessor = RootOperation.ValuesPostProcessor;

			if (rootOperationValuesPostProcessor is not null)
			{
				this.ValuesPostProcessor = (DelegateInvokerFunc2)DelegateInvoker.GetDelegateInvoker(rootOperationValuesPostProcessor);
			}

			var rootOperationConverter = RootOperation.Converter;

			if (rootOperationConverter is not null)
			{
				this.Converter = (DelegateInvokerFunc2)DelegateInvoker.GetDelegateInvoker(rootOperationConverter);
			}

			var rootOperationNullSubstitutor = RootOperation.NullSubstitutor;

			if (rootOperationNullSubstitutor is not null)
			{
				this.NullSubstitutor = (DelegateInvokerFunc0)DelegateInvoker.GetDelegateInvoker(rootOperationNullSubstitutor);
			}

			var rootOperationSourceFilter = RootOperation.SourceFilter;

			if (rootOperationSourceFilter is not null)
			{
				this.SourceFilter = (DelegateInvokerFunc2)DelegateInvoker.GetDelegateInvoker(rootOperationSourceFilter);
			}

			var rootOperationDestinationFilter = RootOperation.DestinationFilter;

			if (rootOperationDestinationFilter is not null)
			{
				this.DestinationFilter = (DelegateInvokerFunc2)DelegateInvoker.GetDelegateInvoker(rootOperationDestinationFilter);
			}
		}
	}

	/// <summary>
	///   Constructs the target.
	/// </summary>
	/// <returns>An object.</returns>
	protected object? ConstructTarget()
	{
		if (TargetConstructor is not null)
		{
			return TargetConstructor.CallFunc();
		}

		return CreateTargetInstance();
	}
}