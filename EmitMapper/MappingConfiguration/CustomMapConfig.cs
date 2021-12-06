namespace EmitMapper.MappingConfiguration;

using System;

using EmitMapper.Conversion;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

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
        if (this.GetMappingOperationFunc == null)
            return new IMappingOperation[0];
        return this.GetMappingOperationFunc(from, to);
    }

    public string GetConfigurationName()
    {
        return this.ConfigurationName;
    }

    public IRootMappingOperation GetRootMappingOperation(Type from, Type to)
    {
        return null;
    }

    #endregion
}