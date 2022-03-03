namespace EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

/// <summary>
///   The dest operation interface.
/// </summary>
public interface IDestOperation : IMappingOperation
{
  /// <summary>
  ///   Gets or Sets the destination.
  /// </summary>
  MemberDescriptor Destination { get; set; }
}