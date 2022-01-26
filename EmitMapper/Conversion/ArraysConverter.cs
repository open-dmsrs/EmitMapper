using System;
using System.Collections.Generic;
using System.Linq;
using EmitMapper.MappingConfiguration;
using EmitMapper.Utils;

namespace EmitMapper.Conversion;

internal class ArraysConverterDifferentTypes<TFrom, TTo> : ICustomConverter
{
  private Func<TFrom, TTo> _converter;

  private MapperDescription _subMapper;

  public void Initialize(Type from, Type to, MapConfigBaseImpl mappingConfig)
  {
    var staticConverters = mappingConfig.GetStaticConvertersManager() ?? StaticConvertersManager.DefaultInstance;
    var staticConverterMethod = staticConverters.GetStaticConverter(Metadata<TFrom>.Type, Metadata<TTo>.Type);
    if (staticConverterMethod != null)
    {
      _converter = (Func<TFrom, TTo>)Delegate.CreateDelegate(
        Metadata<Func<TFrom, TTo>>.Type,
        null,
        staticConverterMethod);
    }
    else
    {
      _subMapper = ObjectMapperManager.DefaultInstance.GetMapperInt(
        Metadata<TFrom>.Type,
        Metadata<TTo>.Type,
        mappingConfig);
      _converter = ConverterBySubmapper;
    }
  }

  public TTo[] Convert(ICollection<TFrom> from, object state)
  {
    if (from == null)
      return null;

    var result = new TTo[from.Count];
    var idx = 0;
    foreach (var f in from)
      result[idx++] = _converter(f);
    return result;
  }

  private TTo ConverterBySubmapper(TFrom from)
  {
    return (TTo)_subMapper.Mapper.Map(from);
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
  // optimized the performance for converting arrays value
  private static readonly Type _converterImplementation = typeof(ArraysConverterOneTypes<>);
  private static readonly Type _Implementation = typeof(ArraysConverterDifferentTypes<,>);

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
        ConverterImplementation = _converterImplementation,
        ConverterClassTypeArguments = tFrom.AsEnumerable()
      };

    return new CustomConverterDescriptor
    {
      ConversionMethodName = "Convert",
      ConverterImplementation = _Implementation,
      ConverterClassTypeArguments = tFrom.AsEnumerable(tTo)
    };
  }
}