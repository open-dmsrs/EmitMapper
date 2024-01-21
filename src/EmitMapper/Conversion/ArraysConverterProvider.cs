namespace EmitMapper.Conversion;

/// <summary>
///   The arrays converter provider.
/// </summary>
internal class ArraysConverterProvider : ICustomConverterProvider
{
	// optimized the performance for converting arrays value
	private static readonly Type ConverterImplementation = typeof(ArraysConverterOneTypes<>);

	private static readonly Type Implementation = typeof(ArraysConverterDifferentTypes<,>);

	/// <summary>
	///   Gets the custom converter descr.
	/// </summary>
	/// <param name="from">The from.</param>
	/// <param name="to">The to.</param>
	/// <param name="mappingConfig">The mapping config.</param>
	/// <returns>A CustomConverterDescriptor.</returns>
	public CustomConverterDescriptor? GetCustomConverterDescr(Type? from, Type? to, MapConfigBaseImpl mappingConfig)
	{
		Type?[]? tFromTypeArgs = DefaultCustomConverterProvider.GetGenericArguments(from);
		Type?[]? tToTypeArgs = DefaultCustomConverterProvider.GetGenericArguments(to);

		if (tFromTypeArgs is null || tToTypeArgs is null || tFromTypeArgs.Length != 1 || tToTypeArgs.Length != 1)
		{
			return default;
		}

		var tFrom = tFromTypeArgs[0];
		var tTo = tToTypeArgs[0];

		if (tFrom == tTo && (tFrom.IsValueType || mappingConfig.GetRootMappingOperation(tFrom, tTo).ShallowCopy))
		{
			return new CustomConverterDescriptor
			{
				ConversionMethodName = "Convert",
				ConverterImplementation = ConverterImplementation,
				ConverterClassTypeArguments = tFrom.AsEnumerable()
			};
		}

		return new CustomConverterDescriptor
		{
			ConversionMethodName = "Convert",
			ConverterImplementation = Implementation,
			ConverterClassTypeArguments = tFrom.AsEnumerable(tTo)
		};
	}
}