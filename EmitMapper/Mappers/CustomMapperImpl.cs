namespace EmitMapper.Mappers;

using System;

using EmitMapper.MappingConfiguration;

public abstract class CustomMapperImpl : ObjectsMapperBaseImpl
{
  protected CustomMapperImpl(
    ObjectMapperManager objectMapperManager,
    Type typeFrom,
    Type typeTo,
    IMappingConfigurator mappingConfigurator,
    object[] storedObjects)
  {
    Initialize(objectMapperManager, typeFrom, typeTo, mappingConfigurator, storedObjects);
  }
}