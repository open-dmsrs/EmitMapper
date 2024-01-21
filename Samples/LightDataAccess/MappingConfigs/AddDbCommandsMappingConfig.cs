namespace LightDataAccess.MappingConfigs;

/// <summary>
///   The add db commands mapping config.
/// </summary>
internal class AddDbCommandsMappingConfig : MapConfigBaseImpl
{
	private readonly string _configName;

	private readonly DbSettings _dbSettings;

	private readonly IEnumerable<string> _excludeFields;

	private readonly IEnumerable<string> _includeFields;

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
		this._dbSettings = dbSettings;
		this._includeFields = includeFields;
		this._excludeFields = excludeFields;
		this._configName = configName;

		if (this._includeFields is not null)
		{
			this._includeFields = this._includeFields.Select(f => f.ToUpper(System.Globalization.CultureInfo.CurrentCulture));
		}

		if (this._excludeFields is not null)
		{
			this._excludeFields = this._excludeFields.Select(f => f.ToUpper(System.Globalization.CultureInfo.CurrentCulture));
		}
	}

	/// <summary>
	///   Gets the configuration name.
	/// </summary>
	/// <returns>A string.</returns>
	public override string GetConfigurationName()
	{
		return _configName;
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

		members = members.Where(m => _includeFields.Contains(m.Name.ToUpper(System.Globalization.CultureInfo.CurrentCulture)) && !_excludeFields.Contains(m.Name.ToUpper(System.Globalization.CultureInfo.CurrentCulture)));

		return members.Select(
		  m => new SrcReadOperation
		  {
			  Source = new MemberDescriptor(m.AsEnumerable()),
			  Setter = (obj, v, s) => ((DbCommand)obj).AddParam(_dbSettings.ParamPrefix + m.Name, v)
		  });
	}
}