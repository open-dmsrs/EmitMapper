using System;
using System.Collections.Generic;
using System.Linq;

namespace EmitMapper.Utils;

public readonly struct TypeDescription : IEquatable<TypeDescription>
{
  public readonly PropertyDescription[] AdditionalProperties;

  public readonly Type Type;

  public TypeDescription(Type type)
    : this(type, Array.Empty<PropertyDescription>())
  {
  }

  public TypeDescription(Type type, IEnumerable<PropertyDescription> additionalProperties)
  {
    Type = type ?? throw new ArgumentNullException(nameof(type));
    if (additionalProperties == null) throw new ArgumentNullException(nameof(additionalProperties));
    AdditionalProperties = additionalProperties.OrderBy(p => p.Name).ToArray();
  }

  public static bool operator ==(in TypeDescription left, in TypeDescription right)
  {
    return left.Equals(right);
  }

  public static bool operator !=(in TypeDescription left, in TypeDescription right)
  {
    return !left.Equals(right);
  }

  public bool Equals(TypeDescription other)
  {
    return Type == other.Type && AdditionalProperties.SequenceEqual(other.AdditionalProperties);
  }

  public override bool Equals(object other)
  {
    return other is TypeDescription description && Equals(description);
  }

  public override int GetHashCode()
  {
    var hashCode = new HashCode();
    hashCode.Add(Type);
    foreach (var property in AdditionalProperties) hashCode.Add(property);
    return hashCode.ToHashCode();
  }
}