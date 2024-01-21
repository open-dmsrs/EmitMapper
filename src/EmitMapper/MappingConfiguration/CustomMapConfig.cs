namespace EmitMapper.MappingConfiguration;

/// <summary>
///   The custom map config.
/// </summary>
public class CustomMapConfig : MapConfigBaseImpl
{
	/// <summary>
	///   Gets or Sets the configuration name.
	/// </summary>
	public string? ConfigurationName { get; set; }

	/// <summary>
	///   Gets or Sets the get mapping operation func.
	/// </summary>
	public Func<Type, Type, IEnumerable<IMappingOperation>>? GetMappingOperationFunc { get; set; }

	/// <summary>
	///   Gets the configuration name.
	/// </summary>
	/// <returns>A string.</returns>
	public override string GetConfigurationName()
	{
		return ConfigurationName;
	}

	/// <summary>
	///   Gets the mapping operations.
	/// </summary>
	/// <param name="from">The from.</param>
	/// <param name="to">The to.</param>
	/// <returns><![CDATA[IEnumerable<IMappingOperation>]]></returns>
	public override IEnumerable<IMappingOperation> GetMappingOperations(Type from, Type to)
	{
		if (GetMappingOperationFunc is null)
		{
			return Array.Empty<IMappingOperation>();
		}

		return GetMappingOperationFunc(from, to);
	}

	/// <summary>
	///   Gets the root mapping operation.
	/// </summary>
	/// <param name="from">The from.</param>
	/// <param name="to">The to.</param>
	/// <returns>An IRootMappingOperation.</returns>
	public override IRootMappingOperation? GetRootMappingOperation(Type from, Type to)
	{
		return null;
	}

	/// <summary>
	/// </summary>
	/// <returns></returns>
	public override StaticConvertersManager? GetStaticConvertersManager()
	{
		return null;
	}
}