using System;
using EmitMapper.Conversion;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

namespace EmitMapper.MappingConfiguration;

public class CustomMapConfig : IMappingConfigurator
{
    public Func<Type, Type, IMappingOperation[]> GetMappingOperationFunc { get; set; }

    public string ConfigurationName { get; set; }

    #region IMappingConfigurator Members

    public StaticConvertersManager GetStaticConvertersManager()
    {
        return null;
    }

    #endregion

    #region IMappingConfigurator Members

    public IMappingOperation[] GetMappingOperations(Type from, Type to)
    {
        if (GetMappingOperationFunc == null)
            return Array.Empty<IMappingOperation>();
        return GetMappingOperationFunc(from, to);
    }

    public string GetConfigurationName()
    {
        return ConfigurationName;
    }

    public IRootMappingOperation GetRootMappingOperation(Type from, Type to)
    {
        return null;
    }

    #endregion
}