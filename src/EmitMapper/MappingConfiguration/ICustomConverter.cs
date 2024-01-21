namespace EmitMapper.MappingConfiguration;

/// <summary>
///   The custom converter interface.
/// </summary>
public interface ICustomConverter
{
	/// <summary>
	/// </summary>
	/// <param name="from">The from.</param>
	/// <param name="to">The to.</param>
	/// <param name="mappingConfig">The mapping config.</param>
	void Initialize(Type from, Type to, MapConfigBaseImpl? mappingConfig);
}