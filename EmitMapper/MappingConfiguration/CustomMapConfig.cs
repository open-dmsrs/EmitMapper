using System;
using System.Collections.Generic;
using EmitMapper.Conversion;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

namespace EmitMapper.MappingConfiguration;

public class CustomMapConfig : MapConfigBaseImpl
{
  public Func<Type, Type, IEnumerable<IMappingOperation>> GetMappingOperationFunc { get; set; }

  public string ConfigurationName { get; set; }

  #region IMappingConfigurator Members

  /// <summary>
  /// </summary>
  /// <returns></returns>
  public override StaticConvertersManager GetStaticConvertersManager()
  {
    return null;
  }

  #endregion

  #region IMappingConfigurator Members

  public override IEnumerable<IMappingOperation> GetMappingOperations(Type from, Type to)
  {
    if (GetMappingOperationFunc == null)
      return Array.Empty<IMappingOperation>();
    return GetMappingOperationFunc(from, to);
  }

  public override string GetConfigurationName()
  {
    return ConfigurationName;
  }

  public override IRootMappingOperation GetRootMappingOperation(Type from, Type to)
  {
    return null;
  }

  #endregion
}