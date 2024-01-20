namespace EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

/// <summary>
///   The read write operation interface.
/// </summary>
public interface IReadWriteOperation : IDestWriteOperation, ISrcReadOperation
{
	/// <summary>
	///   Gets or Sets the converter.
	/// </summary>
	Delegate Converter { get; set; }

	/// <summary>
	///   Gets or Sets the destination filter.
	/// </summary>
	Delegate DestinationFilter { get; set; }

	/// <summary>
	///   Gets or Sets the null substitutor.
	/// </summary>
	Delegate NullSubstitutor { get; set; } // generic type: NullSubstitutor

	/// <summary>
	///   Gets or Sets the source filter.
	/// </summary>
	Delegate SourceFilter { get; set; }

	/// <summary>
	///   Gets or Sets the target constructor.
	/// </summary>
	Delegate TargetConstructor { get; set; } // generic type: TargetConstructor
}
//public class ReadWriteOperation : IReadWriteOperation
//{
//  public Delegate NullSubstitutor { get; set; }

//  public Type NullSubstitutorType { get; }
//  public Delegate TargetConstructor { get; set; }
//  public Type TypeTargetConstructor { get; }
//  public Delegate Converter { get; set; }
//  public Type TypeConverter { get; set; }
//  public Delegate DestinationFilter { get; set; }
//  public Type TypeDestinationFilter { get; }

//  public Delegate SourceFilter { get; set; }
//  public Type TypeSourceFilter { get; }

//  public MemberDescriptor Destination { get; set; }
//  public MemberDescriptor Source { get; set; }
//}