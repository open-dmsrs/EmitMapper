namespace EmitMapper.MappingConfiguration;

using System;

public interface ICustomConverter
{
  void Initialize(Type from, Type to, MapConfigBaseImpl mappingConfig);
}