namespace EmitMapper.Utils;

/// <summary>
///   The profile map.
/// </summary>
public class ProfileMap
{
	/// <summary>
	///   Gets a value indicating whether allow null collections.
	/// </summary>
	public bool AllowNullCollections { get; }

	/// <summary>
	///   Gets a value indicating whether allow null destination values.
	/// </summary>
	public bool AllowNullDestinationValues { get; }

	/// <summary>
	///   Gets a value indicating whether constructor mapping enabled.
	/// </summary>
	public bool ConstructorMappingEnabled { get; }

	/// <summary>
	///   Gets a value indicating whether enable null propagation for query mapping.
	/// </summary>
	public bool EnableNullPropagationForQueryMapping { get; }

	/// <summary>
	///   Gets a value indicating whether field mapping enabled.
	/// </summary>
	public bool FieldMappingEnabled { get; }

	/// <summary>
	///   Gets the global ignores.
	/// </summary>
	public IReadOnlyCollection<string>? GlobalIgnores { get; }

	/// <summary>
	///   Gets a value indicating whether method mapping enabled.
	/// </summary>
	public bool MethodMappingEnabled { get; }

	/// <summary>
	///   Gets the name.
	/// </summary>
	public string? Name { get; }

	/// <summary>
	///   Gets the postfixes.
	/// </summary>
	public List<string>? Postfixes { get; }

	/// <summary>
	///   Gets the prefixes.
	/// </summary>
	public List<string>? Prefixes { get; }

	/// <summary>
	///   Gets the should map field.
	/// </summary>
	public Func<FieldInfo, bool>? ShouldMapField { get; }

	/// <summary>
	///   Gets the should map method.
	/// </summary>
	public Func<MethodInfo, bool>? ShouldMapMethod { get; }

	/// <summary>
	///   Gets the should map property.
	/// </summary>
	public Func<PropertyInfo, bool>? ShouldMapProperty { get; }

	/// <summary>
	///   Gets the should use constructor.
	/// </summary>
	public Func<ConstructorInfo, bool>? ShouldUseConstructor { get; }

	/// <summary>
	///   Gets the source extension methods.
	/// </summary>
	public IEnumerable<MethodInfo>? SourceExtensionMethods { get; }
}