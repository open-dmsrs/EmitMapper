using System;
using System.Diagnostics;
using System.Reflection;

namespace EmitMapper.Utils;

[DebuggerDisplay("{Name}-{Type.Name}")]
public readonly struct PropertyDescription : IEquatable<PropertyDescription>
{
  public readonly bool CanWrite;

  public readonly string Name;

  public readonly Type Type;

  public PropertyDescription(string name, Type type, bool canWrite = true)
  {
    Name = name;
    Type = type;
    CanWrite = canWrite;
  }

  public PropertyDescription(PropertyInfo property)
  {
    Name = property.Name;
    Type = property.PropertyType;
    CanWrite = property.CanWrite;
  }

  public static bool operator ==(in PropertyDescription left, in PropertyDescription right)
  {
    return left.Equals(right);
  }

  public static bool operator !=(in PropertyDescription left, in PropertyDescription right)
  {
    return !left.Equals(right);
  }

  public bool Equals(PropertyDescription other)
  {
    return Name == other.Name && Type == other.Type && CanWrite == other.CanWrite;
  }

  public override bool Equals(object other)
  {
    return other is PropertyDescription description && Equals(description);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(Name, Type, CanWrite);
  }
}