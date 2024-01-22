namespace EmitMapper.MappingConfiguration.MappingOperations;

/// <summary>
///   Generates the following code:
///   var tempSrc = Source.member;
///   if(tempSrc == null)
///   {
///   Destination.member = null;
///   }
///   else
///   {
///   var tempDst = Destination.member;
///   if(tempDst == null)
///   {
///   tempDst = new DestinationMemberType();
///   }
///   // Operations:
///   tempDst.fld1 = tempSrc.fld1;
///   tempDst.fld2 = tempSrc.fld2;
///   ...
///   Destination.member = tempDst;
///   }
/// </summary>
public class ReadWriteComplex : IComplexOperation, IReadWriteOperation
{
	/// <summary>
	///   Gets or Sets the converter.
	/// </summary>
	public Delegate? Converter { get; set; }

	/// <summary>
	///   Gets or Sets the destination.
	/// </summary>
	public MemberDescriptor? Destination { get; set; }

	/// <summary>
	///   Gets or Sets the destination filter.
	/// </summary>
	public Delegate? DestinationFilter { get; set; }

	/// <summary>
	///   Gets or Sets the null substitutor.
	/// </summary>
	public Delegate? NullSubstitutor { get; set; }

	/// <summary>
	///   Gets or Sets the operations.
	/// </summary>
	public List<IMappingOperation> Operations { get; set; }

	/// <summary>
	///   Gets or Sets a value indicating whether shallow copy.
	/// </summary>
	public bool ShallowCopy { get; set; }

	/// <summary>
	///   Gets or Sets the source.
	/// </summary>
	public MemberDescriptor? Source { get; set; }

	/// <summary>
	///   Gets or Sets the source filter.
	/// </summary>
	public Delegate? SourceFilter { get; set; }

	/// <summary>
	///   Gets or Sets the target constructor.
	/// </summary>
	public Delegate? TargetConstructor { get; set; }

	/// <summary>
	///   Gets or Sets the values post processor.
	/// </summary>
	public Delegate? ValuesPostProcessor { get; set; }

	/// <summary>
	///   Tos the string.
	/// </summary>
	/// <returns>A string.</returns>
	public override string ToString()
	{
		return "ReadWriteComplex. Source member:" + Source + " Target member:" + Destination;
	}
}