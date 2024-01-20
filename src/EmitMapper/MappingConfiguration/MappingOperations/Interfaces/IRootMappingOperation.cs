namespace EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

/// <summary>
///   The root mapping operation interface.
/// </summary>
public interface IRootMappingOperation : IMappingOperation
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
	///   Gets or Sets the from.
	/// </summary>
	Type From { get; set; }

	/// <summary>
	///   Gets or Sets the null substitutor.
	/// </summary>
	Delegate NullSubstitutor { get; set; }

	/// <summary>
	///   Gets or Sets a value indicating whether shallow copy.
	/// </summary>
	bool ShallowCopy { get; set; }

	/// <summary>
	///   Gets or Sets the source filter.
	/// </summary>
	Delegate SourceFilter { get; set; }

	/// <summary>
	///   Gets or Sets the target constructor.
	/// </summary>
	Delegate TargetConstructor { get; set; }

	/// <summary>
	///   Gets or Sets the to.
	/// </summary>
	Type To { get; set; }

	/// <summary>
	///   Gets or Sets the values post processor.
	/// </summary>
	Delegate ValuesPostProcessor { get; set; }
}