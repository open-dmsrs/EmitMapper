namespace EmitMapper.Conversion;

using System;
using System.Collections.Generic;
using System.Linq;

using EmitMapper.MappingConfiguration;

internal class ArraysConverterDifferentTypes<TFrom, TTo> : ICustomConverter
{
    private Func<TFrom, TTo> _converter;

    private ObjectsMapperDescr _subMapper;

    public void Initialize(Type from, Type to, MapConfigBaseImpl mappingConfig)
    {
        var staticConverters = mappingConfig.GetStaticConvertersManager() ?? StaticConvertersManager.DefaultInstance;
        var staticConverterMethod = staticConverters.GetStaticConverter(typeof(TFrom), typeof(TTo));
        if (staticConverterMethod != null)
        {
            this._converter = (Func<TFrom, TTo>)Delegate.CreateDelegate(
                typeof(Func<TFrom, TTo>),
                null,
                staticConverterMethod);
        }
        else
        {
            this._subMapper = ObjectMapperManager.DefaultInstance.GetMapperInt(
                typeof(TFrom),
                typeof(TTo),
                mappingConfig);
            this._converter = this.ConverterBySubmapper;
        }
    }

    public TTo[] Convert(ICollection<TFrom> from, object state)
    {
        if (from == null)
            return null;

        var result = new TTo[from.Count];
        var idx = 0;
        foreach (var f in from)
            result[idx++] = this._converter(f);
        return result;
    }

    private TTo ConverterBySubmapper(TFrom from)
    {
        return (TTo)this._subMapper.mapper.Map(from);
    }
}

internal class ArraysConverterOneTypes<T>
{
    public T[] Convert(ICollection<T> from, object state)
    {
        return from?.ToArray();
    }
}

internal class ArraysConverterProvider : ICustomConverterProvider
{
    public CustomConverterDescriptor GetCustomConverterDescr(Type from, Type to, MapConfigBaseImpl mappingConfig)
    {
        var tFromTypeArgs = DefaultCustomConverterProvider.GetGenericArguments(from);
        var tToTypeArgs = DefaultCustomConverterProvider.GetGenericArguments(to);
        if (tFromTypeArgs == null || tToTypeArgs == null || tFromTypeArgs.Length != 1 || tToTypeArgs.Length != 1)
            return null;
        var tFrom = tFromTypeArgs[0];
        var tTo = tToTypeArgs[0];
        if (tFrom == tTo && (tFrom.IsValueType || mappingConfig.GetRootMappingOperation(tFrom, tTo).ShallowCopy))
            return new CustomConverterDescriptor
                       {
                           ConversionMethodName = "Convert",
                           ConverterImplementation = typeof(ArraysConverterOneTypes<>),
                           ConverterClassTypeArguments = new[] { tFrom }
                       };

        return new CustomConverterDescriptor
                   {
                       ConversionMethodName = "Convert",
                       ConverterImplementation = typeof(ArraysConverterDifferentTypes<,>),
                       ConverterClassTypeArguments = new[] { tFrom, tTo }
                   };
    }
}