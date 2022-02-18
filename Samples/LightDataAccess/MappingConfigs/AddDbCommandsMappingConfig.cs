using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using EmitMapper;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;
using EmitMapper.Utils;

namespace LightDataAccess.MappingConfigs;

internal class AddDbCommandsMappingConfig : MapConfigBaseImpl
{
  private readonly string _configName;

  private readonly DbSettings _dbSettings;

  private readonly IEnumerable<string> _excludeFields;

  private readonly IEnumerable<string> _includeFields;

  public AddDbCommandsMappingConfig(
    DbSettings dbSettings,
    IEnumerable<string> includeFields,
    IEnumerable<string> excludeFields,
    string configName)
  {
    _dbSettings = dbSettings;
    _includeFields = includeFields;
    _excludeFields = excludeFields;
    _configName = configName;

    if (_includeFields != null) _includeFields = _includeFields.Select(f => f.ToUpper());

    if (_excludeFields != null) _excludeFields = _excludeFields.Select(f => f.ToUpper());
  }

  public override string GetConfigurationName()
  {
    return _configName;
  }

  public override IEnumerable<IMappingOperation> GetMappingOperations(Type from, Type to)
  {
    var members = ReflectionHelper.GetPublicFieldsAndProperties(from);
    if (_includeFields != null)
      members = members.Where(m => _includeFields.Contains(m.Name.ToUpper()));

    if (_excludeFields != null)
      members = members.Where(m => !_excludeFields.Contains(m.Name.ToUpper()));

    return members.Select(
      m => new SrcReadOperation
      {
        Source = new MemberDescriptor(m.AsEnumerable()),
        Setter = (obj, v, s) => ((DbCommand)obj).AddParam(_dbSettings.ParamPrefix + m.Name, v)
      });
  }
}