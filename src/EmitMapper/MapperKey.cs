using System;
using System.Collections.Generic;
using EmitMapper.MappingConfiguration;

namespace EmitMapper;

public readonly struct MapperKey : IEqualityComparer<MapperKey>, IEquatable<MapperKey>
{
  private readonly int _hash;

  private readonly string _mapperTypeName;

  public MapperKey(Type typeFrom, Type typeTo, IMappingConfigurator config, int currentInstantId)
  {
    _mapperTypeName = $"M{currentInstantId}_{typeFrom?.FullName}_{typeTo?.FullName}_{config?.GetConfigurationName()}";
    _hash = HashCode.Combine(typeFrom, typeTo, config, currentInstantId);
  }

  public bool Equals(MapperKey x, MapperKey y)
  {
    return x._mapperTypeName == y._mapperTypeName;
  }

  public bool Equals(MapperKey rhs)
  {
    return _mapperTypeName == rhs._mapperTypeName;
  }

  public override bool Equals(object obj)
  {
    if (obj == null)
      return false;

    var rhs = (MapperKey)obj;

    return _hash == rhs._hash && _mapperTypeName == rhs._mapperTypeName;
  }

  public int GetHashCode(MapperKey obj)
  {
    return obj._hash;
  }

  public override int GetHashCode()
  {
    return _hash;
  }

  public string GetMapperTypeName()
  {
    return _mapperTypeName;
  }

  public override string ToString()
  {
    return _mapperTypeName;
  }
}