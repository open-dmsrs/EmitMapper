using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

namespace EmitMapper.MappingConfiguration.MappingOperations;
/// <summary>
/// The src read operation.
/// </summary>

public class SrcReadOperation : ISrcReadOperation
{
  /// <summary>
  /// Gets or Sets the setter.
  /// </summary>
  public ValueSetter Setter { get; set; }

  /// <summary>
  /// Gets or Sets the source.
  /// </summary>
  public MemberDescriptor Source { get; set; }

  /// <summary>
  /// Tos the string.
  /// </summary>
  /// <returns>A string.</returns>
  public override string ToString()
  {
    return "SrcReadOperation. Source member:" + Source;
  }
}