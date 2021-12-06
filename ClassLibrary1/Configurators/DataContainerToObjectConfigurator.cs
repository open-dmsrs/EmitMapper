using System;
using System.Linq;
using System.Reflection;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using Extendsoft.HMIS.DataSync.FileProcess;

namespace Extendsoft.HMIS.DataSync.Mapping.Configurators
{
    /// <summary>
    ///     The data container to object configuration.
    /// </summary>
    public class DataContainerToObjectConfigurator : MapConfigBase<DataContainerToObjectConfigurator>
    {
        /// <summary>
        ///     Gets the mapping operations.
        /// </summary>
        /// <param name="from">The type from.</param>
        /// <param name="to">To type to.</param>
        /// <returns>The mapping operations.</returns>
        public override IMappingOperation[] GetMappingOperations(Type from, Type to)
        {
            return FilterOperations(from, to, ReflectionUtils.GetTypeDataContainerDescription(to)
                                                             .Select(fieldsDescription =>
                                                                         {
                                                                             string fieldName = fieldsDescription.Key;
                                                                             MemberInfo destinationMember =
                                                                                 fieldsDescription.Value.Item1;
                                                                             Type fieldType =
                                                                                 fieldsDescription.Value.Item2;
                                                                             return new DestWriteOperation
                                                                                        {
                                                                                            Destination =
                                                                                                new MemberDescriptor(
                                                                                                destinationMember),
                                                                                            Getter =
                                                                                                (ValueGetter<object>)
                                                                                                ((item, state) =>
                                                                                                     {
                                                                                                         if (item ==
                                                                                                             null ||
                                                                                                             !(item is
                                                                                                               DataContainer))
                                                                                                         {
                                                                                                             return
                                                                                                                 ValueToWrite
                                                                                                                     <
                                                                                                                         object
                                                                                                                         >
                                                                                                                     .Skip
                                                                                                                     ();
                                                                                                         }

                                                                                                         var container =
                                                                                                             item as
                                                                                                             DataContainer;
                                                                                                         string value;
                                                                                                         if (
                                                                                                             container
                                                                                                                 .Fields ==
                                                                                                             null ||
                                                                                                             !container
                                                                                                                  .Fields
                                                                                                                  .TryGetValue
                                                                                                                  (fieldName,
                                                                                                                   out
                                                                                                                       value))
                                                                                                         {
                                                                                                             return
                                                                                                                 ValueToWrite
                                                                                                                     <
                                                                                                                         object
                                                                                                                         >
                                                                                                                     .Skip
                                                                                                                     ();
                                                                                                         }
                                                                                                         Type
                                                                                                             destinationType
                                                                                                                 =
                                                                                                                 EmitMapper
                                                                                                                     .Utils
                                                                                                                     .ReflectionUtils
                                                                                                                     .GetMemberType
                                                                                                                     (destinationMember);
                                                                                                         object
                                                                                                             destinationMemberValue
                                                                                                                 =
                                                                                                                 ReflectionUtils
                                                                                                                     .ConvertValue
                                                                                                                     (value,
                                                                                                                      fieldType,
                                                                                                                      destinationType);

                                                                                                         return
                                                                                                             ValueToWrite
                                                                                                                 <object
                                                                                                                     >
                                                                                                                 .ReturnValue
                                                                                                                 (destinationMemberValue);
                                                                                                     })
                                                                                        };
                                                                         })).ToArray();
        }
    }
}