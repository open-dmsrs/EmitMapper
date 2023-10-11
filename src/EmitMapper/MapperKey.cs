namespace EmitMapper;

public readonly struct MapperKey : IEqualityComparer<MapperKey>, IEquatable<MapperKey>
{
  private readonly int _hash;

  private readonly string _mapperTypeName;

  /// <summary>
  /// Initializes a new instance of the <see cref="MapperKey"/> class.
  /// </summary>
  /// <param name="typeFrom">The type from.</param>
  /// <param name="typeTo">The type to.</param>
  /// <param name="config">The config.</param>
  /// <param name="currentInstantId">The current instant id.</param>
  public MapperKey(Type typeFrom, Type typeTo, IMappingConfigurator config, int currentInstantId)
  {
    _mapperTypeName = $"M{currentInstantId}_{typeFrom?.FullName}_{typeTo?.FullName}_{config?.GetConfigurationName()}";
    _hash = HashCode.Combine(typeFrom, typeTo, config, currentInstantId);
  }

  /// <summary>
  /// Equals the.
  /// </summary>
  /// <param name="x">The x.</param>
  /// <param name="y">The y.</param>
  /// <returns>A bool.</returns>
  public bool Equals(MapperKey x, MapperKey y)
  {
    return x._mapperTypeName == y._mapperTypeName;
  }

  /// <summary>
  /// Equals the.
  /// </summary>
  /// <param name="rhs">The rhs.</param>
  /// <returns>A bool.</returns>
  public bool Equals(MapperKey rhs)
  {
    return _mapperTypeName == rhs._mapperTypeName;
  }

  /// <summary>
  /// Equals the.
  /// </summary>
  /// <param name="obj">The obj.</param>
  /// <returns>A bool.</returns>
  public override bool Equals(object obj)
  {
    if (obj == null)
      return false;

    var rhs = (MapperKey)obj;

    return _hash == rhs._hash && _mapperTypeName == rhs._mapperTypeName;
  }

  /// <summary>
  /// Gets the hash code.
  /// </summary>
  /// <param name="obj">The obj.</param>
  /// <returns>An int.</returns>
  public int GetHashCode(MapperKey obj)
  {
    return obj._hash;
  }

  /// <summary>
  /// Gets the hash code.
  /// </summary>
  /// <returns>An int.</returns>
  public override int GetHashCode()
  {
    return _hash;
  }

  /// <summary>
  /// Gets the mapper type name.
  /// </summary>
  /// <returns>A string.</returns>
  public string GetMapperTypeName()
  {
    return _mapperTypeName;
  }

  /// <summary>
  /// Tos the string.
  /// </summary>
  /// <returns>A string.</returns>
  public override string ToString()
  {
    return _mapperTypeName;
  }
}