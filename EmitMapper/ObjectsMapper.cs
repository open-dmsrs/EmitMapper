using System.Collections.Generic;
using EmitMapper.Mappers;

namespace EmitMapper;

public class ObjectsMapper<TFrom, TTo>
{
  private readonly ObjectsMapperBaseImpl _mapperImpl;

  public ObjectsMapper(ObjectsMapperBaseImpl mapperImpl)
  {
    _mapperImpl = mapperImpl;
  }

  public TTo Map(TFrom from, TTo to, object state)
  {
    return (TTo)_mapperImpl.Map(from, to, state);
  }

  public TTo Map(TFrom from, TTo to)
  {
    return (TTo)_mapperImpl.Map(from, to, null);
  }

  public TTo Map(TFrom from)
  {
    return (TTo)_mapperImpl.Map(from);
  }

  public IEnumerable<TTo> MapEnum(IEnumerable<TFrom> sourceCollection)
  {
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

  public TTo MapUsingState(TFrom from, object state)
  {
    return (TTo)_mapperImpl.Map(from, null, state);
  }
}