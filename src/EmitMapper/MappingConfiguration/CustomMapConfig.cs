using System;
using System.Collections.Generic;
using EmitMapper.Conversion;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

namespace EmitMapper.MappingConfiguration;

public class CustomMapConfig : MapConfigBaseImpl
{
  public string ConfigurationName { get; set; }

  public Func<Type, Type, IEnumerable<IMappingOperation>> GetMappingOperationFunc { get; set; }

  public override string GetConfigurationName()
  {
    return ConfigurationName;
  }

  public override IEnumerable<IMappingOperation> GetMappingOperations(Type from, Type to)
  {
    if (GetMappingOperationFunc == null)
      return Array.Empty<IMappingOperation>();

    return GetMappingOperationFunc(from, to);
  }

  public override IRootMappingOperation GetRootMappingOperation(Type from, Type to)
  {
    return null;
  }

  /// <summary>
  /// </summary>
  /// <returns></returns>
  public override StaticConvertersManager GetStaticConvertersManager()
  {
    return null;
  }
}