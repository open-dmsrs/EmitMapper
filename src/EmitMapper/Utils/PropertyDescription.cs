namespace EmitMapper.Utils;

[DebuggerDisplay("{Name}-{Type.Name}")]
public readonly struct PropertyDescription : IEquatable<PropertyDescription>
{
	public readonly bool CanWrite;

	public readonly string Name;

	public readonly Type Type;

	/// <summary>
	/// Initializes a new instance of the <see cref="PropertyDescription"/> struct.
	/// Initializes a new instance of the <see cref="PropertyDescription"/> class.
	/// </summary>
	/// <param name="name">The name.</param>
	/// <param name="type">The type.</param>
	/// <param name="canWrite">If true, can write.</param>
	public PropertyDescription(string name, Type type, bool canWrite = true)
	{
		Name = name;
		Type = type;
		CanWrite = canWrite;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="PropertyDescription"/> struct.
	/// Initializes a new instance of the <see cref="PropertyDescription"/> class.
	/// </summary>
	/// <param name="property">The property.</param>
	public PropertyDescription(PropertyInfo property)
	{
		Name = property.Name;
		Type = property.PropertyType;
		CanWrite = property.CanWrite;
	}

	public static bool operator !=(in PropertyDescription left, in PropertyDescription right)
	{
		return !left.Equals(right);
	}

	public static bool operator ==(in PropertyDescription left, in PropertyDescription right)
	{
		return left.Equals(right);
	}

	/// <summary>
	/// Equals the.
	/// </summary>
	/// <param name="other">The other.</param>
	/// <returns>A bool.</returns>
	public bool Equals(PropertyDescription other)
	{
		return Name == other.Name && Type == other.Type && CanWrite == other.CanWrite;
	}

	/// <summary>
	/// Equals the.
	/// </summary>
	/// <param name="other">The other.</param>
	/// <returns>A bool.</returns>
	public override bool Equals(object other)
	{
		return other is PropertyDescription description && Equals(description);
	}

	/// <summary>
	/// Gets the hash code.
	/// </summary>
	/// <returns>An int.</returns>
	public override int GetHashCode()
	{
		return HashCode.Combine(Name, Type, CanWrite);
	}
}