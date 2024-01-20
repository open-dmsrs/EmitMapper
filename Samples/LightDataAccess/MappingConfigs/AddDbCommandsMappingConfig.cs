namespace LightDataAccess.MappingConfigs;

/// <summary>
///   The add db commands mapping config.
/// </summary>
internal class AddDbCommandsMappingConfig : MapConfigBaseImpl
{
	private readonly string configName;

	private readonly DbSettings dbSettings;

	private readonly IEnumerable<string> excludeFields;

	private readonly IEnumerable<string> includeFields;

	/// <summary>
	///   Initializes a new instance of the <see cref="AddDbCommandsMappingConfig" /> class.
	/// </summary>
	/// <param name="dbSettings">The db settings.</param>
	/// <param name="includeFields">The include fields.</param>
	/// <param name="excludeFields">The exclude fields.</param>
	/// <param name="configName">The config name.</param>
	public AddDbCommandsMappingConfig(
	  DbSettings dbSettings,
	  IEnumerable<string> includeFields,
	  IEnumerable<string> excludeFields,
	  string configName)
	{
		this.dbSettings = dbSettings;
		this.includeFields = includeFields;
		this.excludeFields = excludeFields;
		this.configName = configName;

		if (this.includeFields != null)
		{
			this.includeFields = this.includeFields.Select(f => f.ToUpper());
		}

		if (this.excludeFields != null)
		{
			this.excludeFields = this.excludeFields.Select(f => f.ToUpper());
		}
	}

	/// <summary>
	///   Gets the configuration name.
	/// </summary>
	/// <returns>A string.</returns>
	public override string GetConfigurationName()
	{
		return configName;
	}

	/// <summary>
	///   Gets the mapping operations.
	/// </summary>
	/// <param name="from">The from.</param>
	/// <param name="to">The to.</param>
	/// <returns><![CDATA[IEnumerable<IMappingOperation>]]></returns>
	public override IEnumerable<IMappingOperation> GetMappingOperations(Type from, Type to)
	{
		var members = ReflectionHelper.GetPublicFieldsAndProperties(from);

		if (includeFields != null)
		{
			members = members.Where(m => includeFields.Contains(m.Name.ToUpper()));
		}

		if (excludeFields != null)
		{
			members = members.Where(m => !excludeFields.Contains(m.Name.ToUpper()));
		}

		return members.Select(
		  m => new SrcReadOperation
		  {
			  Source = new MemberDescriptor(m.AsEnumerable()),
			  Setter = (obj, v, s) => ((DbCommand)obj).AddParam(dbSettings.ParamPrefix + m.Name, v)
		  });
	}
}