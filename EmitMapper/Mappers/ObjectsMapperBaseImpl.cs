namespace EmitMapper.Mappers;

using System;

using EmitMapper.EmitInvoker.Delegates;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

/// <summary>
///     Base class for Mappers
/// </summary>
public abstract class ObjectsMapperBaseImpl
{
    // public IMappingConfigurator MappingConfigurator => this._mappingConfigurator;

    /// <summary>
    ///     Copies object properties and members of "from" to object "to"
    /// </summary>
    /// <param name="from">Source object</param>
    /// <param name="to">Destination object</param>
    /// <returns>Destination object</returns>
    public virtual object Map(object from, object to, object state)
    {
        object result;

        if (this.SourceFilter != null)
            if (!(bool)this.SourceFilter.CallFunc(from, state))
                return to;

        if (this.DestinationFilter != null)
            if (!(bool)this.DestinationFilter.CallFunc(to, state))
                return to;

        if (from == null)
        {
            result = this.NullSubstitutor == null ? null : this.NullSubstitutor.CallFunc();
        }
        else if (this.Converter != null)
        {
            result = this.Converter.CallFunc(from, state);
        }
        else
        {
            if (to == null)
                to = this.ConstructTarget();

            result = this.MapImpl(from, to, state);
        }

        if (this.ValuesPostProcessor != null)
            result = this.ValuesPostProcessor.CallFunc(result, state);

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
    internal ObjectMapperManager ObjectMapperMannager;

    /// <summary>
    ///     Type of source object
    /// </summary>
    internal Type TypeFrom;

    /// <summary>
    ///     Type of destination object
    /// </summary>
    internal Type TypeTo;

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

    public IMappingConfigurator MappingConfigurator;

    protected IRootMappingOperation RootOperation;

    public object[] StroredObjects;

    protected DelegateInvokerFunc0 TargetConstructor;

    protected DelegateInvokerFunc0 NullSubstitutor;

    protected DelegateInvokerFunc2 Converter;

    protected DelegateInvokerFunc2 ValuesPostProcessor;

    protected DelegateInvokerFunc2 DestinationFilter;

    protected DelegateInvokerFunc2 SourceFilter;

    internal void Initialize(
        ObjectMapperManager objectMapperMannager,
        Type typeFrom,
        Type typeTo,
        IMappingConfigurator mappingConfigurator,
        object[] stroredObjects)
    {
        this.ObjectMapperMannager = objectMapperMannager;
        this.TypeFrom = typeFrom;
        this.TypeTo = typeTo;
        this.MappingConfigurator = mappingConfigurator;
        this.StroredObjects = stroredObjects;
        if (this.MappingConfigurator != null)
        {
            this.RootOperation = this.MappingConfigurator.GetRootMappingOperation(typeFrom, typeTo);
            if (this.RootOperation == null)
                this.RootOperation = new RootMappingOperation(typeFrom, typeTo);

            var constructor = this.RootOperation.TargetConstructor;
            if (constructor != null)
                this.TargetConstructor = (DelegateInvokerFunc0)DelegateInvoker.GetDelegateInvoker(constructor);

            var valuesPostProcessor = this.RootOperation.ValuesPostProcessor;
            if (valuesPostProcessor != null)
                this.ValuesPostProcessor =
                    (DelegateInvokerFunc2)DelegateInvoker.GetDelegateInvoker(valuesPostProcessor);

            var converter = this.RootOperation.Converter;
            if (converter != null)
                this.Converter = (DelegateInvokerFunc2)DelegateInvoker.GetDelegateInvoker(converter);

            var nullSubstitutor = this.RootOperation.NullSubstitutor;
            if (nullSubstitutor != null)
                this.NullSubstitutor = (DelegateInvokerFunc0)DelegateInvoker.GetDelegateInvoker(nullSubstitutor);

            var sourceFilter = this.RootOperation.SourceFilter;
            if (sourceFilter != null)
                this.SourceFilter = (DelegateInvokerFunc2)DelegateInvoker.GetDelegateInvoker(sourceFilter);

            var destinationFilter = this.RootOperation.DestinationFilter;
            if (destinationFilter != null)
                this.DestinationFilter = (DelegateInvokerFunc2)DelegateInvoker.GetDelegateInvoker(destinationFilter);
        }
    }

    protected object ConstructTarget()
    {
        if (this.TargetConstructor != null)
            return this.TargetConstructor.CallFunc();
        return this.CreateTargetInstance();
    }

    #endregion
}