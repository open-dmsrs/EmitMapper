namespace EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

/// <summary>
///   The complex operation interface.
/// </summary>
internal interface IComplexOperation : IMappingOperation
{
	/// <summary>
	///   Gets or Sets the operations.
	/// </summary>
	List<IMappingOperation> Operations { get; set; }
}