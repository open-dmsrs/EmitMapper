namespace LightDataAccess.Configurators;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;
using EmitMapper.Utils;

/// <summary>
///   The data container to object configuration.
/// </summary>
public class DataContainerToEntityPropertyMappingConfigurator : DefaultMapConfig
{
  /// <summary>
  ///   Gets the mapping operations.
  /// </summary>
  /// <param name="from">The type from.</param>
  /// <param name="to">To type to.</param>
  /// <returns>The mapping operations.</returns>
  public override IEnumerable<IMappingOperation> GetMappingOperations(Type from, Type to)
  {
    return FilterOperations(
      from,
      to,
      ReflectionHelper.GetPublicFieldsAndProperties(to)
        .Where(
          member => (member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property)
                    && ((PropertyInfo)member).GetSetMethod() != null).Select(
          destinationMember => (IMappingOperation)new DestWriteOperation
                                                    {
                                                      Destination = new MemberDescriptor(destinationMember),
                                                      Getter = (ValueGetter<object>)((item, state) =>
                                                              {
                                                                if (item is not DataContainer value)
                                                                  return ValueToWrite<object>.Skip();
                                                                var destinationType =
                                                                  ReflectionHelper.GetMemberReturnType(
                                                                    destinationMember);

                                                                var fieldDescription =
                                                                  ReflectionHelper.GetDataMemberDefinition(
                                                                    destinationMember);
                                                                var destinationMemberValue =
                                                                  ConvertFieldToDestinationProperty(
                                                                    value,
                                                                    destinationType,
                                                                    fieldDescription.FirstOrDefault());

                                                                return destinationMemberValue == null
                                                                         ? ValueToWrite<object>.Skip()
                                                                         : ValueToWrite<object>.ReturnValue(
                                                                           destinationMemberValue);
                                                              })
                                                    }));
  }

  /// <summary>
  ///   Converts the field to destination property.
  /// </summary>
  /// <param name="container">The container.</param>
  /// <param name="destinationType">The destination type.</param>
  /// <param name="fieldDescription">The field description.</param>
  /// <returns>
  ///   The conversion result.
  /// </returns>
  private static object ConvertFieldToDestinationProperty(
    DataContainer container,
    Type destinationType,
    Tuple<string, Type> fieldDescription)
  {
    if (container == null || container.Fields == null || string.IsNullOrEmpty(fieldDescription.Item1)) return null;

    string sourceValue;
    if (!container.Fields.TryGetValue(fieldDescription.Item1, out sourceValue) || sourceValue == null) return null;

    var sourceType = fieldDescription.Item2 ?? sourceValue.GetType();
    return ReflectionHelper.ConvertValue(sourceValue, sourceType, destinationType);
  }
}