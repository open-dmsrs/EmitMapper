using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.Utils;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

namespace Extendsoft.HMIS.DataSync.Mapping.Configurators
{
    /// <summary>
    ///     The object to data container configuration.
    /// </summary>
    public class ObjectToDataContainerConfigurator : MapConfigBase<ObjectToDataContainerConfigurator>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ObjectToDataContainerConfigurator" /> class.
        /// </summary>
        public ObjectToDataContainerConfigurator()
        {
            ConstructBy(() => new DataContainer {Fields = new Dictionary<string, string>()});
        }

        /// <summary>
        ///     Gets the mapping operations.
        /// </summary>
        /// <param name="from">The type from.</param>
        /// <param name="to">To type to.</param>
        /// <returns>The mapping operations.</returns>
        public override IMappingOperation[] GetMappingOperations(Type from, Type to)
        {
            return FilterOperations(from, to, ReflectionUtils.GetTypeDataContainerDescription(from)
                                                             .Select(fieldsDescription =>
                                                                         {
                                                                             string fieldName = fieldsDescription.Key;
                                                                             MemberInfo sourceMember =
                                                                                 fieldsDescription.Value.Item1;
                                                                             Type fieldType =
                                                                                 fieldsDescription.Value.Item2;
                                                                             return new SrcReadOperation
                                                                                        {
                                                                                            Source =
                                                                                                new MemberDescriptor(
                                                                                                sourceMember),
                                                                                            Setter =
                                                                                                (destination, value,
                                                                                                 state) =>
                                                                                                    {
                                                                                                        if (
                                                                                                            destination ==
                                                                                                            null ||
                                                                                                            value ==
                                                                                                            null ||
                                                                                                            !(destination
                                                                                                              is
                                                                                                              DataContainer))
                                                                                                        {
                                                                                                            return;
                                                                                                        }

                                                                                                        var container =
                                                                                                            destination
                                                                                                            as
                                                                                                            DataContainer;

                                                                                                        // var sourceType = EmitMapper.Utils.ReflectionUtils.GetMemberType(sourceMember);
                                                                                                        // var destinationMemberValue = ReflectionUtils.ConvertValue(value, sourceType, fieldType);
                                                                                                        string
                                                                                                            destinationMemberValue
                                                                                                                =
                                                                                                                value ==
                                                                                                                null
                                                                                                                    ? null
                                                                                                                    : value
                                                                                                                          .ToString
                                                                                                                          ();
                                                                                                        container.Fields
                                                                                                                 .Add(
                                                                                                                     fieldName,
                                                                                                                     destinationMemberValue);
                                                                                                    }
                                                                                        };
                                                                         })).ToArray();
        }
    }
}