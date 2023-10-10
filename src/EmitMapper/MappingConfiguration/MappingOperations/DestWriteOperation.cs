namespace EmitMapper.MappingConfiguration.MappingOperations;

/// <summary>
///   The dest write operation.
/// </summary>
public class DestWriteOperation : IDestWriteOperation
{
  /// <summary>
  ///   Gets or Sets the destination.
  /// </summary>
  public MemberDescriptor Destination { get; set; }

  /// <summary>
  ///   Gets or Sets the getter.
  /// </summary>
  public Delegate Getter { get; set; }

  /// <summary>
  ///   Tos the string.
  /// </summary>
  /// <returns>A string.</returns>
  public override string ToString()
  {
    return "DestWriteOperation. Target member:" + Destination;
  }
}