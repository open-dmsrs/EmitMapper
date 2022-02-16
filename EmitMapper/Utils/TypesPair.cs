namespace EmitMapper.Utils;

using System;
using System.Collections.Generic;

public readonly struct TypesPair : IEqualityComparer<TypesPair>, IEquatable<TypesPair>
{
  public readonly Type DestinationType;

  public readonly Type SourceType;

  private readonly int _hash;

  public TypesPair(Type typeFrom, Type typeTo)
  {
    SourceType = typeFrom;
    DestinationType = typeTo;
    _hash = HashCode.Combine(typeFrom, typeTo);
  }

  public bool ContainsGenericParameters =>
    SourceType.ContainsGenericParameters || DestinationType.ContainsGenericParameters;

  public bool IsConstructedGenericType =>
    SourceType.IsConstructedGenericType || DestinationType.IsConstructedGenericType;

  public bool IsGenericTypeDefinition => SourceType.IsGenericTypeDefinition || DestinationType.IsGenericTypeDefinition;

  public static bool operator ==(in TypesPair left, in TypesPair right)
  {
    return left.Equals(right);
  }

  public static bool operator !=(in TypesPair left, in TypesPair right)
  {
    return !left.Equals(right);
  }

  public TypesPair CloseGenericTypes(in TypesPair closedTypes)
  {
    var sourceArguments = closedTypes.SourceType.GenericTypeArguments;
    var destinationArguments = closedTypes.DestinationType.GenericTypeArguments;
    if (sourceArguments.Length == 0)
      sourceArguments = destinationArguments;
    else if (destinationArguments.Length == 0) destinationArguments = sourceArguments;
    var closedSourceType = SourceType.IsGenericTypeDefinition
                             ? SourceType.MakeGenericType(sourceArguments)
                             : SourceType;
    var closedDestinationType = DestinationType.IsGenericTypeDefinition
                                  ? DestinationType.MakeGenericType(destinationArguments)
                                  : DestinationType;
    return new TypesPair(closedSourceType, closedDestinationType);
  }

  public bool Equals(TypesPair x, TypesPair y)
  {
    return x.Equals(y);
  }

  public bool Equals(TypesPair other)
  {
    return _hash == other._hash && SourceType == other.SourceType && DestinationType == other.DestinationType;
  }

  public override bool Equals(object obj)
  {
    return Equals((TypesPair)obj);
  }

  public int GetHashCode(TypesPair obj)
  {
    return obj._hash;
  }

  public override int GetHashCode()
  {
    return _hash;
  }

  public TypesPair GetTypeDefinitionIfGeneric()
  {
    return new TypesPair(GetTypeDefinitionIfGeneric(SourceType), GetTypeDefinitionIfGeneric(DestinationType));
  }

  public override string ToString()
  {
    return SourceType + " -> " + DestinationType;
  }

  private static Type GetTypeDefinitionIfGeneric(Type type)
  {
    return type.IsGenericType ? type.GetGenericTypeDefinitionCache() : type;
  }
}