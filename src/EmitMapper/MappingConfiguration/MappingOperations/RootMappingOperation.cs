namespace EmitMapper.MappingConfiguration.MappingOperations;

/// <summary>
///   The root mapping operation.
/// </summary>
/// <remarks>
///   Initializes a new instance of the <see cref="RootMappingOperation" /> class.
/// </remarks>
/// <param name="from">The from.</param>
/// <param name="to">The to.</param>
public class RootMappingOperation(Type from, Type to) : IRootMappingOperation
{
	/// <summary>
	///   Gets or Sets the converter.
	/// </summary>
	public Delegate? Converter { get; set; }

	/// <summary>
	///   Gets or Sets the destination filter.
	/// </summary>
	public Delegate? DestinationFilter { get; set; }

	/// <summary>
	///   Gets or Sets the from.
	/// </summary>
	public Type From { get; set; } = from;

	/// <summary>
	///   Gets or Sets the null substitutor.
	/// </summary>
	public Delegate? NullSubstitutor { get; set; }

	/// <summary>
	///   Gets or Sets a value indicating whether shallow copy.
	/// </summary>
	public bool ShallowCopy { get; set; }

	/// <summary>
	///   Gets or Sets the source filter.
	/// </summary>
	public Delegate? SourceFilter { get; set; }

	/// <summary>
	///   Gets or Sets the target constructor.
	/// </summary>
	public Delegate? TargetConstructor { get; set; }

	/// <summary>
	///   Gets or Sets the to.
	/// </summary>
	public Type To { get; set; } = to;

	/// <summary>
	///   Gets or Sets the values post processor.
	/// </summary>
	public Delegate? ValuesPostProcessor { get; set; }
}