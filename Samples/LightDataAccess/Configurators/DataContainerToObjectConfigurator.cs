using System;
using System.Collections.Generic;
using System.Linq;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;
using EmitMapper.Utils;

namespace LightDataAccess.Configurators;

/// <summary>
///   The data container to object configuration.
/// </summary>
public class DataContainerToObjectConfigurator : MapConfigBaseImpl
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
        ReflectionUtils.GetTypeDataContainerDescription(to)
          .Select(
            fieldsDescription =>
            {
              var fieldName = fieldsDescription.Key;
              var destinationMember =
                fieldsDescription.Value.Item1;
              var fieldType =
                fieldsDescription.Value.Item2;
              return new DestWriteOperation
              {
                Destination =
                  new MemberDescriptor(destinationMember),
                Getter = (ValueGetter<object>)((item, state) =>
                {
                  if (item is not DataContainer container)
                    return ValueToWrite<object>.Skip();
                  if (container.Fields == null ||
                      !container.Fields.TryGetValue(fieldName, out var value))
                    return ValueToWrite<object>.Skip();
                  var destinationType = ReflectionUtils.GetMemberReturnType(destinationMember);
                  var destinationMemberValue = ReflectionUtils.ConvertValue(
                    value,
                    fieldType,
                    destinationType);
                  return ValueToWrite<object>.ReturnValue(destinationMemberValue);
                })
              };
            }
          )
      )
      .ToArray();
  }
}