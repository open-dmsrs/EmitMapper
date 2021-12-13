namespace EmitMapper.Mappers;

using System;

using EmitMapper.MappingConfiguration;

public abstract class CustomMapperImpl : ObjectsMapperBaseImpl
{
    public CustomMapperImpl(
        ObjectMapperManager mapperMannager,
        Type typeFrom,
        Type typeTo,
        IMappingConfigurator mappingConfigurator,
        object[] storedObjects)
    {
        this.Initialize(mapperMannager, typeFrom, typeTo, mappingConfigurator, storedObjects);
    }
}