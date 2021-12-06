using System;

namespace EmitMapper.MappingConfiguration
{
    public interface ICustomConverter
    {
        void Initialize(Type from, Type to, MapConfigBaseImpl mappingConfig);
    }
}
