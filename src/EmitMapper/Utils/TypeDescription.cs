namespace EmitMapper.Utils;

public readonly struct TypeDescription : IEquatable<TypeDescription>
{
	public PropertyDescription[] AdditionalProperties { get; }

	public Type Type { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="TypeDescription"/> struct.
	/// </summary>
	/// <param name="type">The type.</param>
	public TypeDescription(Type type)
	  : this(type, Array.Empty<PropertyDescription>())
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TypeDescription"/> struct.
	/// Initializes a new instance of the <see cref="TypeDescription"/> class.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="additionalProperties">The additional properties.</param>
	public TypeDescription(Type type, IEnumerable<PropertyDescription> additionalProperties)
	{
		this.Type = type ?? throw new ArgumentNullException(nameof(type));

		ArgumentNullException.ThrowIfNull(additionalProperties);

		additionalProperties = additionalProperties.OrderBy(p => p.Name).ToArray();
	}

	public static bool operator !=(in TypeDescription left, in TypeDescription right)
	{
		return !left.Equals(right);
	}

	public static bool operator ==(in TypeDescription left, in TypeDescription right)
	{
		return left.Equals(right);
	}

	/// <summary>
	/// Equals the.
	/// </summary>
	/// <param name="other">The other.</param>
	/// <returns>A bool.</returns>
	public bool Equals(TypeDescription other)
	{
		return Type == other.Type && AdditionalProperties.SequenceEqual(other.AdditionalProperties);
	}

	/// <summary>
	/// Equals the.
	/// </summary>
	/// <param name="other">The other.</param>
	/// <returns>A bool.</returns>
	public override bool Equals(object? other)
	{
		return other is TypeDescription description && Equals(description);
	}

	/// <summary>
	/// Gets the hash code.
	/// </summary>
	/// <returns>An int.</returns>
	public override int GetHashCode()
	{
		var hashCode = default(HashCode);
		hashCode.Add(Type);
		foreach (var property in AdditionalProperties)
		{
			hashCode.Add(property);
		}

		return hashCode.ToHashCode();
	}
}