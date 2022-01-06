using System;
using System.Collections.Generic;

namespace EmitMapper.Utils;

public struct TypesPair : IEqualityComparer<TypesPair>, IEquatable<TypesPair>
{
  public readonly Type TypeFrom;

  public readonly Type TypeTo;

  private readonly int _hash;

  public TypesPair(Type typeFrom, Type typeTo)
  {
    TypeFrom = typeFrom;
    TypeTo = typeTo;

    _hash = HashCode.Combine(typeFrom, typeTo);
  }

  public bool Equals(TypesPair x, TypesPair y)
  {
    return x.Equals(y);
  }

  public int GetHashCode(TypesPair obj)
  {
    return obj.GetHashCode();
  }

  public bool Equals(TypesPair other)
  {
    return _hash == other._hash && TypeFrom == other.TypeFrom && TypeTo == other.TypeTo;
  }

  public override int GetHashCode()
  {
    return _hash;
  }

  public override bool Equals(object obj)
  {
    return Equals((TypesPair)obj);
  }

  public override string ToString()
  {
    return TypeFrom + " -> " + TypeTo;
  }
}