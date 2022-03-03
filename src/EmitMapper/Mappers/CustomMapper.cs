using System;
using EmitMapper.MappingConfiguration;

namespace EmitMapper.Mappers;

/// <summary>
///   The custom mapper.
/// </summary>
public abstract class CustomMapper : MapperBase
{
  /// <summary>
  ///   Initializes a new instance of the <see cref="CustomMapper" /> class.
  /// </summary>
  /// <param name="objectMapperManager">The object mapper manager.</param>
  /// <param name="typeFrom">The type from.</param>
  /// <param name="typeTo">The type to.</param>
  /// <param name="mappingConfigurator">The mapping configurator.</param>
  /// <param name="storedObjects">The stored objects.</param>
  protected CustomMapper(
    Mapper objectMapperManager,
    Type typeFrom,
    Type typeTo,
    IMappingConfigurator mappingConfigurator,
    object[] storedObjects)
  {
    Initialize(objectMapperManager, typeFrom, typeTo, mappingConfigurator, storedObjects);
  }
}