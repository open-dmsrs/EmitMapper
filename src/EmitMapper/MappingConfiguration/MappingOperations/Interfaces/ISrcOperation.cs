namespace EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

/// <summary>
///   The src operation interface.
/// </summary>
public interface ISrcOperation : IMappingOperation
{
  /// <summary>
  ///   Gets or Sets the source.
  /// </summary>
  MemberDescriptor Source { get; set; }
}