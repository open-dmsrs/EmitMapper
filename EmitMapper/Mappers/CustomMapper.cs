using System;
using EmitMapper.MappingConfiguration;

namespace EmitMapper.Mappers;

public abstract class CustomMapper : MapperBase
{
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