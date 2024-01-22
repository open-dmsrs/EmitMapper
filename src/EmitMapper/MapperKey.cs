namespace EmitMapper;

public readonly struct MapperKey : IEqualityComparer<MapperKey>, IEquatable<MapperKey>
{
	private readonly int hash;

	private readonly string mapperTypeName;

	/// <summary>
	/// Initializes a new instance of the <see cref="MapperKey"/> struct.
	/// Initializes a new instance of the <see cref="MapperKey"/> class.
	/// </summary>
	/// <param name="typeFrom">The type from.</param>
	/// <param name="typeTo">The type to.</param>
	/// <param name="config">The config.</param>
	/// <param name="currentInstantId">The current instant id.</param>
	public MapperKey(Type typeFrom, Type typeTo, IMappingConfigurator? config, int currentInstantId)
	{
		mapperTypeName = $"M{currentInstantId}_{typeFrom?.FullName}_{typeTo?.FullName}_{config?.GetConfigurationName()}";
		hash = HashCode.Combine(typeFrom, typeTo, config, currentInstantId);
	}

	/// <summary>
	/// Equals the.
	/// </summary>
	/// <param name="x">The x.</param>
	/// <param name="y">The y.</param>
	/// <returns>A bool.</returns>
	public bool Equals(MapperKey x, MapperKey y)
	{
		return x.mapperTypeName == y.mapperTypeName;
	}

	/// <summary>
	/// Equals the.
	/// </summary>
	/// <param name="rhs">The rhs.</param>
	/// <returns>A bool.</returns>
	public bool Equals(MapperKey rhs)
	{
		return mapperTypeName == rhs.mapperTypeName;
	}

	/// <summary>
	/// Equals the.
	/// </summary>
	/// <param name="obj">The obj.</param>
	/// <returns>A bool.</returns>
	public override bool Equals(object? obj)
	{
		if (obj is null)
		{
			return false;
		}

		var rhs = (MapperKey)obj;

		return hash == rhs.hash && mapperTypeName == rhs.mapperTypeName;
	}

	/// <summary>
	/// Gets the hash code.
	/// </summary>
	/// <param name="obj">The obj.</param>
	/// <returns>An int.</returns>
	public int GetHashCode(MapperKey obj)
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
	/// Gets the mapper type name.
	/// </summary>
	/// <returns>A string.</returns>
	public string GetMapperTypeName()
	{
		return mapperTypeName;
	}

	/// <summary>
	/// Tos the string.
	/// </summary>
	/// <returns>A string.</returns>
	public override string ToString()
	{
		return mapperTypeName;
	}
}