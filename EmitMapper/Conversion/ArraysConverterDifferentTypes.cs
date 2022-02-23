using System;
using System.Collections.Generic;
using EmitMapper.MappingConfiguration;
using EmitMapper.Utils;

namespace EmitMapper.Conversion;

internal class ArraysConverterDifferentTypes<TFrom, TTo> : ICustomConverter
{
  private Func<TFrom, TTo> _converter;

  private MapperDescription _subMapper;

  public TTo[] Convert(ICollection<TFrom> from, object state)
  {
    if (from == default)
      return default;

    var result = new TTo[from.Count];
    var idx = 0;

    foreach (var f in from)
      result[idx++] = _converter(f);

    return result;
  }

  public void Initialize(Type from, Type to, MapConfigBaseImpl mappingConfig)
  {
    var staticConverters = mappingConfig.GetStaticConvertersManager() ?? StaticConvertersManager.DefaultInstance;
    var staticConverterMethod = staticConverters.GetStaticConverter(Metadata<TFrom>.Type, Metadata<TTo>.Type);

    if (staticConverterMethod != default)
    {
      _converter = (Func<TFrom, TTo>)Delegate.CreateDelegate(
        Metadata<Func<TFrom, TTo>>.Type,
        default,
        staticConverterMethod);
    }
    else
    {
      _subMapper = Mapper.Default.GetMapperDescription(
        Metadata<TFrom>.Type,
        Metadata<TTo>.Type,
        mappingConfig);

      _converter = ConverterBySubmapper;
    }
  }

  private TTo ConverterBySubmapper(TFrom from)
  {
    return (TTo)_subMapper.Mapper.Map(from);
  }
}