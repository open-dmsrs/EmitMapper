using System;
using EmitMapper.MappingConfiguration.MappingOperations;

namespace EmitMapper.MappingConfiguration;

public abstract class MapConfigBase<TDerived> : MapConfigBaseImpl
    where TDerived : class
{
    private TDerived _derived;

    public void Init(TDerived derived)
    {
        _derived = derived;
    }

    /// <summary>
    ///     Define custom type converter
    /// </summary>
    /// <typeparam name="TFrom">Source type</typeparam>
    /// <typeparam name="To">Destination type</typeparam>
    /// <param name="converter">Function which converts an instance of the source type to an instance of the destination type</param>
    /// <returns></returns>
    public new TDerived ConvertUsing<TFrom, To>(Func<TFrom, To> converter)
    {
        return (TDerived)base.ConvertUsing(converter);
    }

    /// <summary>
    ///     Define conversion for a generic. It is able to convert not one particular class but all generic family
    ///     providing a generic converter.
    /// </summary>
    /// <param name="from">Type of source. Can be also generic class or abstract array.</param>
    /// <param name="to">Type of destination. Can be also generic class or abstract array.</param>
    /// <param name="converterProvider">
    ///     Provider for getting detailed information about generic conversion.
    /// </param>
    /// <returns></returns>
    public new TDerived ConvertGeneric(Type from, Type to, ICustomConverterProvider converterProvider)
    {
        return (TDerived)base.ConvertGeneric(from, to, converterProvider);
    }

    /// <summary>
    ///     Setup function which returns value for destination if appropriate source member is null.
    /// </summary>
    /// <typeparam name="TFrom">Type of source member</typeparam>
    /// <typeparam name="TTo">Type of destination member</typeparam>
    /// <param name="nullSubstitutor">Function which returns value for destination if appropriate source member is null</param>
    /// <returns></returns>
    public new TDerived NullSubstitution<TFrom, TTo>(Func<object, TTo> nullSubstitutor)
    {
        return (TDerived)base.NullSubstitution<TFrom, TTo>(nullSubstitutor);
    }

    /// <summary>
    ///     Define members which should be ignored
    /// </summary>
    /// <param name="typeFrom">Source type for which ignore members are defining</param>
    /// <param name="typeTo">Destination type for which ignore members are defining</param>
    /// <param name="ignoreNames">Array of member names which should be ignored</param>
    /// <returns></returns>
    public new TDerived IgnoreMembers(Type typeFrom, Type typeTo, string[] ignoreNames)
    {
        return (TDerived)base.IgnoreMembers(typeFrom, typeTo, ignoreNames);
    }

    /// <summary>
    ///     Define members which should be ignored
    /// </summary>
    /// <typeparam name="TFrom">Source type for which ignore members are defining</typeparam>
    /// <typeparam name="TTo">Destination type for which ignore members are defining</typeparam>
    /// <param name="ignoreNames">Array of member names which should be ignored</param>
    /// <returns></returns>
    public new TDerived IgnoreMembers<TFrom, TTo>(string[] ignoreNames)
    {
        return (TDerived)base.IgnoreMembers<TFrom, TTo>(ignoreNames);
    }

    /// <summary>
    ///     Define a custom constructor for the specified type
    /// </summary>
    /// <typeparam name="T">Type for which constructor is defining</typeparam>
    /// <param name="constructor">Custom constructor</param>
    /// <returns></returns>
    public new TDerived ConstructBy<T>(TargetConstructor<T> constructor)
    {
        return (TDerived)base.ConstructBy(constructor);
    }

    /// <summary>
    ///     Define postprocessor for specified type
    /// </summary>
    /// <typeparam name="T">Objects of this type and all it's descendants will be postprocessed</typeparam>
    /// <param name="postProcessor"></param>
    /// <returns></returns>
    public new TDerived PostProcess<T>(ValuesPostProcessor<T> postProcessor)
    {
        return (TDerived)base.PostProcess(postProcessor);
    }

    /// <summary>
    ///     Set unique configuration name to force Emit Mapper create new mapper instead using appropriate cached one.
    /// </summary>
    /// <returns></returns>
    public new TDerived SetConfigName(string configurationName)
    {
        return (TDerived)base.SetConfigName(configurationName);
    }

    public new TDerived FilterDestination<T>(ValuesFilter<T> valuesFilter)
    {
        return (TDerived)base.FilterDestination(valuesFilter);
    }

    public new TDerived FilterSource<T>(ValuesFilter<T> valuesFilter)
    {
        return (TDerived)base.FilterSource(valuesFilter);
    }
}