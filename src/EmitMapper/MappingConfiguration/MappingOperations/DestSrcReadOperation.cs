namespace EmitMapper.MappingConfiguration.MappingOperations;

/// <summary>
///   The dest src read operation.
/// </summary>
public class DestSrcReadOperation : IDestReadOperation, ISrcReadOperation
{
	/// <summary>
	///   Gets or Sets the destination.
	/// </summary>
	public MemberDescriptor? Destination { get; set; }

	/// <summary>
	///   Gets or Sets the source.
	/// </summary>
	public MemberDescriptor? Source { get; set; }

	/// <summary>
	///   Gets or Sets the value processor.
	/// </summary>
	public ValueProcessor? ValueProcessor { get; set; }

	/// <summary>
	///   Tos the string.
	/// </summary>
	/// <returns>A string.</returns>
	public override string ToString()
	{
		return "DestSrcReadOperation. Source member:" + Source + " Target member:" + Destination;
	}
}