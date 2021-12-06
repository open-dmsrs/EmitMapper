namespace EmitMapper.Mappers;

using System;

using EmitMapper.EmitInvoker.Delegates;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

/// <summary>
///     Base class for Mappers
/// </summary>
public abstract class ObjectsMapperBaseImpl
{
    public IMappingConfigurator MappingConfigurator => this._mappingConfigurator;

    /// <summary>
    ///     Copies object properties and members of "from" to object "to"
    /// </summary>
    /// <param name="from">Source object</param>
    /// <param name="to">Destination object</param>
    /// <returns>Destination object</returns>
    public virtual object Map(object from, object to, object state)
    {
        object result;

        if (this._sourceFilter != null)
            if (!(bool)this._sourceFilter.CallFunc(from, state))
                return to;

        if (this._destinationFilter != null)
            if (!(bool)this._destinationFilter.CallFunc(to, state))
                return to;

        if (from == null)
        {
            result = this._nullSubstitutor == null ? null : this._nullSubstitutor.CallFunc();
        }
        else if (this._converter != null)
        {
            result = this._converter.CallFunc(from, state);
        }
        else
        {
            if (to == null)
                to = this.ConstructTarget();

            result = this.MapImpl(from, to, state);
        }

        if (this._valuesPostProcessor != null)
            result = this._valuesPostProcessor.CallFunc(result, state);

        return result;
    }

    /// <summary>
    ///     Creates new instance of destination object and initializes it by values from "from" object
    /// </summary>
    /// <param name="from">source object</param>
    /// <returns></returns>
    public virtual object Map(object from)
    {
        if (from == null)
            return null;
        var to = this.ConstructTarget();
        return this.Map(from, to, null);
    }

    #region Non-public members

    /// <summary>
    ///     Mapper manager
    /// </summary>
    internal ObjectMapperManager mapperMannager;

    /// <summary>
    ///     Type of source object
    /// </summary>
    internal Type typeFrom;

    /// <summary>
    ///     Type of destination object
    /// </summary>
    internal Type typeTo;

    /// <summary>
    ///     True, if reference properties and members of same type should
    ///     be copied by reference (shallow copy, without creating new instance for destination object)
    /// </summary>
    internal bool ShallowCopy = true;

    /// <summary>
    ///     Copies object properties and members of "from" to object "to"
    /// </summary>
    /// <param name="from">Source object</param>
    /// <param name="to">Destination object</param>
    /// <returns>Destination object</returns>
    public abstract object MapImpl(object from, object to, object state);

    /// <summary>
    ///     Creates an instance of destination object
    /// </summary>
    /// <returns>Destination object</returns>
    public abstract object CreateTargetInstance();

    protected IMappingConfigurator _mappingConfigurator;

    protected IRootMappingOperation _rootOperation;

    public object[] StroredObjects;

    protected DelegateInvokerFunc_0 _targetConstructor;

    protected DelegateInvokerFunc_0 _nullSubstitutor;

    protected DelegateInvokerFunc_2 _converter;

    protected DelegateInvokerFunc_2 _valuesPostProcessor;

    protected DelegateInvokerFunc_2 _destinationFilter;

    protected DelegateInvokerFunc_2 _sourceFilter;

    internal void Initialize(
        ObjectMapperManager MapperMannager,
        Type TypeFrom,
        Type TypeTo,
        IMappingConfigurator mappingConfigurator,
        object[] stroredObjects)
    {
        this.mapperMannager = MapperMannager;
        this.typeFrom = TypeFrom;
        this.typeTo = TypeTo;
        this._mappingConfigurator = mappingConfigurator;
        this.StroredObjects = stroredObjects;
        if (this._mappingConfigurator != null)
        {
            this._rootOperation = this._mappingConfigurator.GetRootMappingOperation(TypeFrom, TypeTo);
            if (this._rootOperation == null)
                this._rootOperation = new RootMappingOperation(TypeFrom, TypeTo);

            var constructor = this._rootOperation.TargetConstructor;
            if (constructor != null)
                this._targetConstructor = (DelegateInvokerFunc_0)DelegateInvoker.GetDelegateInvoker(constructor);

            var valuesPostProcessor = this._rootOperation.ValuesPostProcessor;
            if (valuesPostProcessor != null)
                this._valuesPostProcessor =
                    (DelegateInvokerFunc_2)DelegateInvoker.GetDelegateInvoker(valuesPostProcessor);

            var converter = this._rootOperation.Converter;
            if (converter != null)
                this._converter = (DelegateInvokerFunc_2)DelegateInvoker.GetDelegateInvoker(converter);

            var nullSubstitutor = this._rootOperation.NullSubstitutor;
            if (nullSubstitutor != null)
                this._nullSubstitutor = (DelegateInvokerFunc_0)DelegateInvoker.GetDelegateInvoker(nullSubstitutor);

            var sourceFilter = this._rootOperation.SourceFilter;
            if (sourceFilter != null)
                this._sourceFilter = (DelegateInvokerFunc_2)DelegateInvoker.GetDelegateInvoker(sourceFilter);

            var destinationFilter = this._rootOperation.DestinationFilter;
            if (destinationFilter != null)
                this._destinationFilter = (DelegateInvokerFunc_2)DelegateInvoker.GetDelegateInvoker(destinationFilter);
        }
    }

    protected object ConstructTarget()
    {
        if (this._targetConstructor != null)
            return this._targetConstructor.CallFunc();
        return this.CreateTargetInstance();
    }

    #endregion
}