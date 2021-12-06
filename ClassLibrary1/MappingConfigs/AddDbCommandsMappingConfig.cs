using EmitMapper;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.Utils;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace LightDataAccess.MappingConfigs
{
    internal class AddDbCommandsMappingConfig : IMappingConfigurator
    {
        private readonly DbSettings _dbSettings;
        private readonly IEnumerable<string> _includeFields;
        private readonly IEnumerable<string> _excludeFields;
        private readonly string _configName;

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

            if (_includeFields != null)
            {
                _includeFields = _includeFields.Select(f => f.ToUpper());
            }

            if (_excludeFields != null)
            {
                _excludeFields = _excludeFields.Select(f => f.ToUpper());
            }

        }


        #region IMappingConfigurator Members
        public IRootMappingOperation GetRootMappingOperation(Type from, Type to)
        {
            return null;
        }

        public IMappingOperation[] GetMappingOperations(Type from, Type to)
        {
            System.Reflection.MemberInfo[] members = ReflectionUtils.GetPublicFieldsAndProperties(from);
            if (_includeFields != null)
            {
                members = members
                    .Where(m => _includeFields.Contains(m.Name.ToUpper()))
                    .ToArray();
            }

            if (_excludeFields != null)
            {
                members = members
                    .Where(m => !_excludeFields.Contains(m.Name.ToUpper()))
                    .ToArray();
            }

            return members
                .Select(
                    m => new SrcReadOperation
                    {
                        Source = new MemberDescriptor(new[] { m }),
                        Setter = (obj, v, s) => ((DbCommand)obj).AddParam(_dbSettings.paramPrefix + m.Name, v)
                    }
                )
                .ToArray();
        }

        public string GetConfigurationName()
        {
            return _configName;
        }


        #endregion

        public StaticConvertersManager GetStaticConvertersManager()
        {
            return null;
        }
    }
}
