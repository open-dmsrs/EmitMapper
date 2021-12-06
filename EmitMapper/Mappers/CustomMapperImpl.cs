namespace EmitMapper.Mappers;

using System;

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