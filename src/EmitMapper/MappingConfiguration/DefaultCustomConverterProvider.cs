namespace EmitMapper.MappingConfiguration;

/// <summary>
///   The default custom converter provider.
/// </summary>
public class DefaultCustomConverterProvider : ICustomConverterProvider
{
	private readonly Type converterType;

	/// <summary>
	///   Initializes a new instance of the <see cref="DefaultCustomConverterProvider" /> class.
	/// </summary>
	/// <param name="converterType">The converter type.</param>
	public DefaultCustomConverterProvider(Type converterType)
	{
		this.converterType = converterType;
	}

	/// <summary>
	///   Gets the generic arguments.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>An array of Types</returns>
	public static Type[] GetGenericArguments(Type type)
	{
		switch (type.IsArray)
		{
			case true:
				return new[] { type.GetElementType() };
		}

		switch (type.IsGenericType)
		{
			case true:
				return type.GetGenericArguments();
			default:
				return type.GetInterfacesCache().Where(i => i.IsGenericType).Select(i => i.GetGenericArguments())
					.Where(a => a.Length == 1).Select(a => a[0]).ToArray();
		}
	}

	/// <summary>
	///   Gets the custom converter descr.
	/// </summary>
	/// <param name="from">The from.</param>
	/// <param name="to">The to.</param>
	/// <param name="mappingConfig">The mapping config.</param>
	/// <returns>A CustomConverterDescriptor.</returns>
	public virtual CustomConverterDescriptor GetCustomConverterDescr(Type from, Type to, MapConfigBaseImpl mappingConfig)
	{
		return new CustomConverterDescriptor
		{
			ConverterClassTypeArguments = GetGenericArguments(from).Concat(GetGenericArguments(to)),
			ConverterImplementation = converterType,
			ConversionMethodName = "Convert"
		};
	}
}