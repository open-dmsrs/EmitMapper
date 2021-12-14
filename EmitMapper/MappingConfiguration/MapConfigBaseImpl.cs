namespace EmitMapper.MappingConfiguration;

using System;
using System.Collections.Generic;
using System.Linq;

using EmitMapper.Conversion;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;
using EmitMapper.Utils;

public abstract class MapConfigBaseImpl : IMappingConfigurator
{
    private readonly TypeDictionary<Delegate> _customConstructors = new();

    private readonly TypeDictionary<Delegate> _customConverters = new();

    private readonly TypeDictionary<ICustomConverterProvider> _customConvertersGeneric = new();

    private readonly TypeDictionary<Delegate> _destinationFilters = new();

    private readonly TypeDictionary<List<string>> _ignoreMembers = new();

    private readonly TypeDictionary<Delegate> _nullSubstitutors = new();

    private readonly TypeDictionary<Delegate> _postProcessors = new();

    private readonly TypeDictionary<Delegate> _sourceFilters = new();

    private string _configurationName;

    public MapConfigBaseImpl()
    {
        this.RegisterDefaultCollectionConverters();
    }

    protected static string ToStrEnum<T>(IEnumerable<T> t)
    {
        return t == null ? "" : t.ToCsv("|");
    }

    protected static string ToStr<T>(T t)
        where T : class
    {
        return t == null ? "" : t.ToString();
    }

    public abstract IMappingOperation[] GetMappingOperations(Type from, Type to);

    public virtual void BuildConfigurationName()
    {
        this._configurationName = new[]
                                      {
                                          ToStr(this._customConverters), ToStr(this._nullSubstitutors),
                                          ToStr(this._ignoreMembers), ToStr(this._postProcessors),
                                          ToStr(this._customConstructors)
                                      }.ToCsv(";");
    }

    /// <summary>
    ///     Define custom type converter
    /// </summary>
    /// <typeparam name="TFrom">Source type</typeparam>
    /// <typeparam name="TO">Destination type</typeparam>
    /// <param name="converter">Function which converts an inctance of the source type to an instance of the destination type</param>
    /// <returns></returns>
    public IMappingConfigurator ConvertUsing<TFrom, TO>(Func<TFrom, TO> converter)
    {
        this._customConverters.Add(
            new[] { typeof(TFrom), typeof(TO) },
            (ValueConverter<TFrom, TO>)((v, s) => converter(v)));
        return this;
    }

    /// <summary>
    ///     Define conversion for a generic. It is able to convert not one particular class but all generic family
    ///     providing a generic converter.
    /// </summary>
    /// <param name="from">Type of source. Can be also generic class or abstract array.</param>
    /// <param name="to">Type of destination. Can be also generic class or abstract array.</param>
    /// <param name="converterProvider">Provider for getting detailed information about generic conversion</param>
    /// <returns></returns>
    public IMappingConfigurator ConvertGeneric(Type from, Type to, ICustomConverterProvider converterProvider)
    {
        this._customConvertersGeneric.Add(new[] { from, to }, converterProvider);
        return this;
    }

    /// <summary>
    ///     Setup function which returns value for destination if appropriate source member is null.
    /// </summary>
    /// <typeparam name="TFrom">Type of source member</typeparam>
    /// <typeparam name="TTo">Type of destination member</typeparam>
    /// <param name="nullSubstitutor">Function which returns value for destination if appropriate source member is null</param>
    /// <returns></returns>
    public IMappingConfigurator NullSubstitution<TFrom, TTo>(Func<object, TTo> nullSubstitutor)
    {
        this._nullSubstitutors.Add(new[] { typeof(TFrom), typeof(TTo) }, nullSubstitutor);
        return this;
    }

    /// <summary>
    ///     Define members which should be ingored
    /// </summary>
    /// <param name="typeFrom">Source type for which ignore members are defining</param>
    /// <param name="typeTo">Destination type for which ignore members are defining</param>
    /// <param name="ignoreNames">Array of member names which should be ingored</param>
    /// <returns></returns>
    public IMappingConfigurator IgnoreMembers(Type typeFrom, Type typeTo, string[] ignoreNames)
    {
        var ig = this._ignoreMembers.GetValue(new[] { typeFrom, typeTo });
        if (ig == null)
            this._ignoreMembers.Add(new[] { typeFrom, typeTo }, ignoreNames.ToList());
        else
            ig.AddRange(ignoreNames);
        return this;
    }

    /// <summary>
    ///     Define members which should be ingored
    /// </summary>
    /// <typeparam name="TFrom">Source type for which ignore members are defining</typeparam>
    /// <typeparam name="TTo">Destination type for which ignore members are defining</typeparam>
    /// <param name="ignoreNames">Array of member names which should be ingored</param>
    /// <returns></returns>
    public IMappingConfigurator IgnoreMembers<TFrom, TTo>(string[] ignoreNames)
    {
        return this.IgnoreMembers(typeof(TFrom), typeof(TTo), ignoreNames);
    }

    /// <summary>
    ///     Define a custom constructor for the specified type
    /// </summary>
    /// <typeparam name="T">Type for which constructor is defining</typeparam>
    /// <param name="constructor">Custom constructor</param>
    /// <returns></returns>
    public IMappingConfigurator ConstructBy<T>(TargetConstructor<T> constructor)
    {
        this._customConstructors.Add(new[] { typeof(T) }, constructor);
        return this;
    }

    /// <summary>
    ///     Define postprocessor for specified type
    /// </summary>
    /// <typeparam name="T">Objects of this type and all it's descendants will be postprocessed</typeparam>
    /// <param name="postProcessor"></param>
    /// <returns></returns>
    public IMappingConfigurator PostProcess<T>(ValuesPostProcessor<T> postProcessor)
    {
        this._postProcessors.Add(new[] { typeof(T) }, postProcessor);
        return this;
    }

    /// <summary>
    ///     Set unique configuration name to force Emit Mapper create new mapper instead using appropriate cached one.
    /// </summary>
    /// <param name="configurationName">Configuration name</param>
    /// <returns></returns>
    public IMappingConfigurator SetConfigName(string configurationName)
    {
        this._configurationName = configurationName;
        return this;
    }

    public IMappingConfigurator FilterDestination<T>(ValuesFilter<T> valuesFilter)
    {
        this._destinationFilters.Add(new[] { typeof(T) }, valuesFilter);
        return this;
    }

    public IMappingConfigurator FilterSource<T>(ValuesFilter<T> valuesFilter)
    {
        this._sourceFilters.Add(new[] { typeof(T) }, valuesFilter);
        return this;
    }

    public virtual string GetConfigurationName()
    {
        return this._configurationName;
    }

    public virtual StaticConvertersManager GetStaticConvertersManager()
    {
        return null;
    }

    public virtual IRootMappingOperation GetRootMappingOperation(Type from, Type to)
    {
        var converter = this._customConverters.GetValue(new[] { from, to });
        if (converter == null)
            converter = this.GetGenericConverter(from, to);

        return new RootMappingOperation(from, to)
                   {
                       TargetConstructor = this._customConstructors.GetValue(new[] { to }),
                       NullSubstitutor = this._nullSubstitutors.GetValue(new[] { to }),
                       ValuesPostProcessor = this._postProcessors.GetValue(new[] { to }),
                       Converter = converter,
                       DestinationFilter = this._destinationFilters.GetValue(new[] { to }),
                       SourceFilter = this._sourceFilters.GetValue(new[] { from })
                   };
    }

    protected virtual void RegisterDefaultCollectionConverters()
    {
        this.ConvertGeneric(typeof(ICollection<>), typeof(Array), new ArraysConverterProvider());
    }

    protected IEnumerable<IMappingOperation> FilterOperations(
        Type from,
        Type to,
        IEnumerable<IMappingOperation> operations)
    {
        var result = new List<IMappingOperation>();
        foreach (var op in operations)
        {
            if (op is IReadWriteOperation)
            {
                var o = op as IReadWriteOperation;
                if (this.TestIgnore(from, to, o.Source, o.Destination))
                    continue;

                o.NullSubstitutor =
                    this._nullSubstitutors.GetValue(new[] { o.Source.MemberType, o.Destination.MemberType });
                o.TargetConstructor = this._customConstructors.GetValue(new[] { o.Destination.MemberType });
                o.Converter = this._customConverters.GetValue(new[] { o.Source.MemberType, o.Destination.MemberType });
                if (o.Converter == null)
                    o.Converter = this.GetGenericConverter(o.Source.MemberType, o.Destination.MemberType);
                o.DestinationFilter = this._destinationFilters.GetValue(new[] { o.Destination.MemberType });
                o.SourceFilter = this._sourceFilters.GetValue(new[] { o.Source.MemberType });
            }

            if (op is ReadWriteComplex)
            {
                var o = op as ReadWriteComplex;
                o.ValuesPostProcessor = this._postProcessors.GetValue(new[] { o.Destination.MemberType });
            }

            if (op is IComplexOperation)
            {
                var o = op as IComplexOperation;
                var orw = op as IReadWriteOperation;
                o.Operations = this.FilterOperations(
                    orw == null ? from : orw.Source.MemberType,
                    orw == null ? to : orw.Destination.MemberType,
                    o.Operations).ToList();
            }

            result.Add(op);
        }

        return result;
    }

    private Delegate GetGenericConverter(Type from, Type to)
    {
        var converter = this._customConvertersGeneric.GetValue(new[] { from, to });
        if (converter == null)
            return null;

        var converterDescr = converter.GetCustomConverterDescr(from, to, this);

        if (converterDescr == null)
            return null;

        Type genericConverter;
        if (converterDescr.ConverterClassTypeArguments != null && converterDescr.ConverterClassTypeArguments.Length > 0)
            genericConverter =
                converterDescr.ConverterImplementation.MakeGenericType(converterDescr.ConverterClassTypeArguments);
        else
            genericConverter = converterDescr.ConverterImplementation;

        var mi = genericConverter.GetMethod(converterDescr.ConversionMethodName);

        var converterObj = Activator.CreateInstance(genericConverter);
        if (converterObj is ICustomConverter)
            ((ICustomConverter)converterObj).Initialize(from, to, this);

        return Delegate.CreateDelegate(typeof(Func<,,>).MakeGenericType(from, typeof(object), to), converterObj, mi);
    }

    private bool TestIgnore(Type from, Type to, MemberDescriptor fromDescr, MemberDescriptor toDescr)
    {
        var ignore = this._ignoreMembers.GetValue(new[] { from, to });
        if (ignore != null && (ignore.Contains(fromDescr.MemberInfo.Name) || ignore.Contains(toDescr.MemberInfo.Name)))
            return true;
        return false;
    }
}