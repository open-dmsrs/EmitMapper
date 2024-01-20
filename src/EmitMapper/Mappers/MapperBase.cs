using EmitMapper.EmitInvoker.Delegates;

namespace EmitMapper.Mappers;

/// <summary>
///   Base class for Mappers
/// </summary>
public abstract class MapperBase
{
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
	internal Type? TypeFrom;

	/// <summary>
	///   Type of destination object
	/// </summary>
	internal Type? TypeTo;

	protected DelegateInvokerFunc2? converter;

	protected DelegateInvokerFunc2? destinationFilter;

	protected IMappingConfigurator? mappingConfigurator;

	protected DelegateInvokerFunc0? nullSubstitutor;

	protected IRootMappingOperation? rootOperation;

	protected DelegateInvokerFunc2? sourceFilter;

	protected DelegateInvokerFunc0? targetConstructor;

	protected DelegateInvokerFunc2? valuesPostProcessor;

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

		if (sourceFilter is not null)
		{
			if (!(bool)sourceFilter.CallFunc(from, state))
			{
				return to;
			}
		}

		if (destinationFilter is not null)
		{
			if (!(bool)destinationFilter.CallFunc(to, state))
			{
				return to;
			}
		}

		if (from is null)
		{
			result = nullSubstitutor?.CallFunc();
		}
		else if (converter is not null)
		{
			result = converter.CallFunc(from, state);
		}
		else
		{
			to ??= ConstructTarget();

			result = MapImpl(from, to, state);
		}

		if (valuesPostProcessor is not null)
		{
			result = valuesPostProcessor.CallFunc(result, state);
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
		this.mappingConfigurator = mappingConfigurator;
		StoredObjects = storedObjects;

		if (this.mappingConfigurator is not null)
		{
			rootOperation = this.mappingConfigurator.GetRootMappingOperation(typeFrom, typeTo)
							?? new RootMappingOperation(typeFrom, typeTo);

			var constructor = rootOperation.TargetConstructor;

			if (constructor is not null)
			{
				targetConstructor = (DelegateInvokerFunc0)DelegateInvoker.GetDelegateInvoker(constructor);
			}

			var valuesPostProcessor = rootOperation.ValuesPostProcessor;

			if (valuesPostProcessor is not null)
			{
				this.valuesPostProcessor = (DelegateInvokerFunc2)DelegateInvoker.GetDelegateInvoker(valuesPostProcessor);
			}

			var converter = rootOperation.Converter;

			if (converter is not null)
			{
				this.converter = (DelegateInvokerFunc2)DelegateInvoker.GetDelegateInvoker(converter);
			}

			var nullSubstitutor = rootOperation.NullSubstitutor;

			if (nullSubstitutor is not null)
			{
				this.nullSubstitutor = (DelegateInvokerFunc0)DelegateInvoker.GetDelegateInvoker(nullSubstitutor);
			}

			var sourceFilter = rootOperation.SourceFilter;

			if (sourceFilter is not null)
			{
				this.sourceFilter = (DelegateInvokerFunc2)DelegateInvoker.GetDelegateInvoker(sourceFilter);
			}

			var destinationFilter = rootOperation.DestinationFilter;

			if (destinationFilter is not null)
			{
				this.destinationFilter = (DelegateInvokerFunc2)DelegateInvoker.GetDelegateInvoker(destinationFilter);
			}
		}
	}

	/// <summary>
	///   Constructs the target.
	/// </summary>
	/// <returns>An object.</returns>
	protected object ConstructTarget()
	{
		if (targetConstructor is not null)
		{
			return targetConstructor.CallFunc();
		}

		return CreateTargetInstance();
	}
}