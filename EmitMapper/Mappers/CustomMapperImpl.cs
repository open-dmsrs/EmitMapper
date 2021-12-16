using System;
using EmitMapper.MappingConfiguration;

namespace EmitMapper.Mappers;

public abstract class CustomMapperImpl : ObjectsMapperBaseImpl
{
    public CustomMapperImpl(
        ObjectMapperManager objectMapperManager,
        Type typeFrom,
        Type typeTo,
        IMappingConfigurator mappingConfigurator,
        object[] storedObjects)
    {
        Initialize(objectMapperManager, typeFrom, typeTo, mappingConfigurator, storedObjects);
    }
}