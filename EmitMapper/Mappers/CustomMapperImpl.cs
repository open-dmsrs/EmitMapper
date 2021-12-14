namespace EmitMapper.Mappers;

using System;

using EmitMapper.MappingConfiguration;

public abstract class CustomMapperImpl : ObjectsMapperBaseImpl
{
    public CustomMapperImpl(
        ObjectMapperManager objectMapperManager,
        Type typeFrom,
        Type typeTo,
        IMappingConfigurator mappingConfigurator,
        object[] storedObjects)
    {
        this.Initialize(objectMapperManager, typeFrom, typeTo, mappingConfigurator, storedObjects);
    }
}