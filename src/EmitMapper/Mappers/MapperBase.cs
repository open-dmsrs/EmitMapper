using EmitMapper.EmitInvoker.Delegates;

namespace EmitMapper.Mappers;

/// <summary>
///   Base class for Mappers
/// </summary>
public abstract class MapperBase
{
	public object[] StoredObjects;

	/// <summary>
	///   Mapper manager
	/// </summary>
	internal Mapper Mapper;

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

	protected DelegateInvokerFunc2 Converter;

	protected DelegateInvokerFunc2 DestinationFilter;

	protected IMappingConfigurator MappingConfigurator;

	protected DelegateInvokerFunc0 NullSubstitutor;

	protected IRootMappingOperation RootOperation;

	protected DelegateInvokerFunc2 SourceFilter;

	protected DelegateInvokerFunc0 TargetConstructor;

	protected DelegateInvokerFunc2 ValuesPostProcessor;

	/// <summary>
	///   Creates an instance of destination object
	/// </summary>
	/// <returns>Destination object</returns>
	public abstract object CreateTargetInstance();

	// public IMappingConfigurator MappingConfigurator => this._mappingConfigurator;

	/// <summary>
	///   Copies object properties and members of "from" to object "to"
	/// </summary>
	/// <param name="from">Source object</param>
	/// <param name="to">Destination object</param>
	/// <param name="state"></param>
	/// <returns>Destination object</returns>
	public virtual object Map(object from, object to, object state)
	{
		object result;

		if (SourceFilter is not null)
		{
			if (!(bool)SourceFilter.CallFunc(from, state))
			{
				return to;
			}
		}

		if (DestinationFilter is not null)
		{
			if (!(bool)DestinationFilter.CallFunc(to, state))
			{
				return to;
			}
		}

		if (from is null)
		{
			result = NullSubstitutor?.CallFunc();
		}
		else if (Converter is not null)
		{
			result = Converter.CallFunc(from, state);
		}
		else
		{
			to ??= ConstructTarget();

			result = MapImpl(from, to, state);
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
	/// <returns></returns>
	public virtual object? Map(object from)
	{
		if (from is null)
		{
			return null;
		}

		return Map(from, ConstructTarget(), null);
	}

	/// <summary>
	///   Copies object properties and members of "from" to object "to"
	/// </summary>
	/// <param name="from">Source object</param>
	/// <param name="to">Destination object</param>
	/// <param name="state"></param>
	/// <returns>Destination object</returns>
	public abstract object MapImpl(object from, object to, object state);

	/// <summary>
	/// </summary>
	/// <param name="objectMapperManager">The object mapper manager.</param>
	/// <param name="typeFrom">The type from.</param>
	/// <param name="typeTo">The type to.</param>
	/// <param name="mappingConfigurator">The mapping configurator.</param>
	/// <param name="storedObjects">The stored objects.</param>
	internal void Initialize(
	  Mapper objectMapperManager,
	  Type typeFrom,
	  Type typeTo,
	  IMappingConfigurator mappingConfigurator,
	  object[] storedObjects)
	{
		Mapper = objectMapperManager;
		TypeFrom = typeFrom;
		TypeTo = typeTo;
		MappingConfigurator = mappingConfigurator;
		StoredObjects = storedObjects;

		if (MappingConfigurator is not null)
		{
			RootOperation = MappingConfigurator.GetRootMappingOperation(typeFrom, typeTo)
							?? new RootMappingOperation(typeFrom, typeTo);

			var constructor = RootOperation.TargetConstructor;

			if (constructor is not null)
			{
				TargetConstructor = (DelegateInvokerFunc0)DelegateInvoker.GetDelegateInvoker(constructor);
			}

			var valuesPostProcessor = RootOperation.ValuesPostProcessor;

			if (valuesPostProcessor is not null)
			{
				ValuesPostProcessor = (DelegateInvokerFunc2)DelegateInvoker.GetDelegateInvoker(valuesPostProcessor);
			}

			var converter = RootOperation.Converter;

			if (converter is not null)
			{
				Converter = (DelegateInvokerFunc2)DelegateInvoker.GetDelegateInvoker(converter);
			}

			var nullSubstitutor = RootOperation.NullSubstitutor;

			if (nullSubstitutor is not null)
			{
				NullSubstitutor = (DelegateInvokerFunc0)DelegateInvoker.GetDelegateInvoker(nullSubstitutor);
			}

			var sourceFilter = RootOperation.SourceFilter;

			if (sourceFilter is not null)
			{
				SourceFilter = (DelegateInvokerFunc2)DelegateInvoker.GetDelegateInvoker(sourceFilter);
			}

			var destinationFilter = RootOperation.DestinationFilter;

			if (destinationFilter is not null)
			{
				DestinationFilter = (DelegateInvokerFunc2)DelegateInvoker.GetDelegateInvoker(destinationFilter);
			}
		}
	}

	/// <summary>
	///   Constructs the target.
	/// </summary>
	/// <returns>An object.</returns>
	protected object ConstructTarget()
	{
		if (TargetConstructor is not null)
		{
			return TargetConstructor.CallFunc();
		}

		return CreateTargetInstance();
	}
}