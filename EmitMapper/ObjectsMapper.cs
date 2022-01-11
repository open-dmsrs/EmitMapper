using System.Collections.Generic;
using EmitMapper.Mappers;

namespace EmitMapper;

public class ObjectsMapper<TFrom, TTo>
{
  public ObjectsMapper(ObjectsMapperBaseImpl mapperImpl)
  {
    MapperImpl = mapperImpl;
  }

  public ObjectsMapperBaseImpl MapperImpl { get; set; }

  public TTo Map(TFrom from, TTo to, object state)
  {
    return (TTo)MapperImpl.Map(from, to, state);
  }

  public TTo Map(TFrom from, TTo to)
  {
    return (TTo)MapperImpl.Map(from, to, null);
  }

  public TTo Map(TFrom from)
  {
    return (TTo)MapperImpl.Map(from);
  }

  public TTo MapUsingState(TFrom from, object state)
  {
    return (TTo)MapperImpl.Map(from, null, state);
  }

  public IEnumerable<TTo> MapEnum(IEnumerable<TFrom> sourceCollection)
  {
    if (sourceCollection is List<TFrom>)
    {
    }

    foreach (var src in sourceCollection)
      yield return Map(src);
  }

  public List<TTo> MapEnum(List<TFrom> sourceCollection)
  {
    var result = new List<TTo>(sourceCollection.Count);
    foreach (var src in sourceCollection)
      result.Add(Map(src));
    return result;
  }
}