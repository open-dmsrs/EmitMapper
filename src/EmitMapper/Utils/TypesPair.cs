namespace EmitMapper.Utils;

public readonly struct TypesPair : IEqualityComparer<TypesPair>, IEquatable<TypesPair>
{
	public readonly Type? DestinationType;

	public readonly Type? SourceType;

	private readonly int hash;

	/// <summary>
	/// Initializes a new instance of the <see cref="TypesPair"/> struct.
	/// Initializes a new instance of the <see cref="TypesPair"/> class.
	/// </summary>
	/// <param name="typeFrom">The type from.</param>
	/// <param name="typeTo">The type to.</param>
	public TypesPair(Type? typeFrom, Type? typeTo)
	{
		SourceType = typeFrom;
		DestinationType = typeTo;
		hash = HashCode.Combine(typeFrom, typeTo);
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

	/// <summary>
	/// Closes the generic types.
	/// </summary>
	/// <param name="closedTypes">The closed types.</param>
	/// <returns>A TypesPair.</returns>
	public TypesPair CloseGenericTypes(in TypesPair closedTypes)
	{
		var sourceArguments = closedTypes.SourceType.GenericTypeArguments;
		var destinationArguments = closedTypes.DestinationType.GenericTypeArguments;

		if (sourceArguments.Length == 0)
		{
			sourceArguments = destinationArguments;
		}
		else if (destinationArguments.Length == 0)
		{
			destinationArguments = sourceArguments;
		}

		var closedSourceType = SourceType.IsGenericTypeDefinition
		  ? SourceType.MakeGenericType(sourceArguments)
		  : SourceType;

		var closedDestinationType = DestinationType.IsGenericTypeDefinition
		  ? DestinationType.MakeGenericType(destinationArguments)
		  : DestinationType;

		return new TypesPair(closedSourceType, closedDestinationType);
	}

	/// <summary>
	/// Equals the.
	/// </summary>
	/// <param name="x">The x.</param>
	/// <param name="y">The y.</param>
	/// <returns>A bool.</returns>
	public bool Equals(TypesPair x, TypesPair y)
	{
		return x.Equals(y);
	}

	/// <summary>
	/// Equals the.
	/// </summary>
	/// <param name="other">The other.</param>
	/// <returns>A bool.</returns>
	public bool Equals(TypesPair other)
	{
		return hash == other.hash && SourceType == other.SourceType && DestinationType == other.DestinationType;
	}

	/// <summary>
	/// Equals the.
	/// </summary>
	/// <param name="obj">The obj.</param>
	/// <returns>A bool.</returns>
	public override bool Equals(object obj)
	{
		return Equals((TypesPair)obj);
	}

	/// <summary>
	/// Gets the hash code.
	/// </summary>
	/// <param name="obj">The obj.</param>
	/// <returns>An int.</returns>
	public int GetHashCode(TypesPair obj)
	{
		return obj.hash;
	}

	/// <summary>
	/// Gets the hash code.
	/// </summary>
	/// <returns>An int.</returns>
	public override int GetHashCode()
	{
		return hash;
	}

	/// <summary>
	/// Gets the type definition if generic.
	/// </summary>
	/// <returns>A TypesPair.</returns>
	public TypesPair GetTypeDefinitionIfGeneric()
	{
		return new TypesPair(GetTypeDefinitionIfGeneric(SourceType), GetTypeDefinitionIfGeneric(DestinationType));
	}

	/// <summary>
	/// Tos the string.
	/// </summary>
	/// <returns>A string.</returns>
	public override string ToString()
	{
		return SourceType + " -> " + DestinationType;
	}

	/// <summary>
	///   Gets the type definition if generic.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>A Type.</returns>
	private static Type? GetTypeDefinitionIfGeneric(Type? type)
	{
		return type.IsGenericType ? type.GetGenericTypeDefinitionCache() : type;
	}
}