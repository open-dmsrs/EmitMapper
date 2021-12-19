using System;
using EmitMapper.EmitInvoker.Delegates;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

namespace EmitMapper.Mappers;

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
    /// <param name="state"></param>
    /// <returns>Destination object</returns>
    public virtual object Map(object from, object to, object state)
    {
        object result;

        if (SourceFilter != null)
            if (!(bool)SourceFilter.CallFunc(from, state))
                return to;

        if (DestinationFilter != null)
            if (!(bool)DestinationFilter.CallFunc(to, state))
                return to;

        if (from == null)
        {
            result = NullSubstitutor?.CallFunc();
        }
        else if (Converter != null)
        {
            result = Converter.CallFunc(from, state);
        }
        else
        {
            if (to == null)
                to = ConstructTarget();

            result = MapImpl(from, to, state);
        }

        if (ValuesPostProcessor != null)
            result = ValuesPostProcessor.CallFunc(result, state);

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
        var to = ConstructTarget();
        return Map(from, to, null);
    }

    #region Non-public members

    /// <summary>
    ///     Mapper manager
    /// </summary>
    internal ObjectMapperManager ObjectMapperManager;

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
    /// <param name="state"></param>
    /// <returns>Destination object</returns>
    public abstract object MapImpl(object from, object to, object state);

    /// <summary>
    ///     Creates an instance of destination object
    /// </summary>
    /// <returns>Destination object</returns>
    public abstract object CreateTargetInstance();

    protected IMappingConfigurator MappingConfigurator;

    protected IRootMappingOperation RootOperation;

    public object[] StoredObjects;

    protected DelegateInvokerFunc0 TargetConstructor;

    protected DelegateInvokerFunc0 NullSubstitutor;

    protected DelegateInvokerFunc2 Converter;

    protected DelegateInvokerFunc2 ValuesPostProcessor;

    protected DelegateInvokerFunc2 DestinationFilter;

    protected DelegateInvokerFunc2 SourceFilter;

    internal void Initialize(
        ObjectMapperManager objectMapperManager,
        Type typeFrom,
        Type typeTo,
        IMappingConfigurator mappingConfigurator,
        object[] storedObjects)
    {
        ObjectMapperManager = objectMapperManager;
        TypeFrom = typeFrom;
        TypeTo = typeTo;
        MappingConfigurator = mappingConfigurator;
        StoredObjects = storedObjects;
        if (MappingConfigurator != null)
        {
            RootOperation = MappingConfigurator.GetRootMappingOperation(typeFrom, typeTo);
            if (RootOperation == null)
                RootOperation = new RootMappingOperation(typeFrom, typeTo);

            var constructor = RootOperation.TargetConstructor;
            if (constructor != null)
                TargetConstructor = (DelegateInvokerFunc0)DelegateInvoker.GetDelegateInvoker(constructor);

            var valuesPostProcessor = RootOperation.ValuesPostProcessor;
            if (valuesPostProcessor != null)
                ValuesPostProcessor =
                    (DelegateInvokerFunc2)DelegateInvoker.GetDelegateInvoker(valuesPostProcessor);

            var converter = RootOperation.Converter;
            if (converter != null)
                Converter = (DelegateInvokerFunc2)DelegateInvoker.GetDelegateInvoker(converter);

            var nullSubstitutor = RootOperation.NullSubstitutor;
            if (nullSubstitutor != null)
                NullSubstitutor = (DelegateInvokerFunc0)DelegateInvoker.GetDelegateInvoker(nullSubstitutor);

            var sourceFilter = RootOperation.SourceFilter;
            if (sourceFilter != null)
                SourceFilter = (DelegateInvokerFunc2)DelegateInvoker.GetDelegateInvoker(sourceFilter);

            var destinationFilter = RootOperation.DestinationFilter;
            if (destinationFilter != null)
                DestinationFilter = (DelegateInvokerFunc2)DelegateInvoker.GetDelegateInvoker(destinationFilter);
        }
    }

    protected object ConstructTarget()
    {
        if (TargetConstructor != null)
            return TargetConstructor.CallFunc();
        return CreateTargetInstance();
    }

    #endregion
}