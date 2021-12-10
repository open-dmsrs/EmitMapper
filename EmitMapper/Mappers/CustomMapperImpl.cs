namespace EmitMapper.Mappers;

using System;

using EmitMapper.MappingConfiguration;

public abstract class CustomMapperImpl : ObjectsMapperBaseImpl
{
    public CustomMapperImpl(
        ObjectMapperManager mapperMannager,
        Type TypeFrom,
        Type TypeTo,
        IMappingConfigurator mappingConfigurator,
        object[] storedObjects)
    {
        this.Initialize(mapperMannager, TypeFrom, TypeTo, mappingConfigurator, storedObjects);
    }
}