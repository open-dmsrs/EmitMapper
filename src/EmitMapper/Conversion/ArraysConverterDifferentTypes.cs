namespace EmitMapper.Conversion;

/// <summary>
/// The arrays converter different types.
/// </summary>
/// <typeparam name="TFrom"></typeparam>
/// <typeparam name="TTo"></typeparam>
internal class ArraysConverterDifferentTypes<TFrom, TTo> : ICustomConverter
{
	private Func<TFrom, TTo>? converter;

	private MapperDescription? subMapper;

	/// <summary>
	/// Converts the an array of TTos.
	/// </summary>
	/// <param name="from">The from.</param>
	/// <param name="state">The state.</param>
	/// <returns>An array of TTos</returns>
	public TTo[]? Convert(ICollection<TFrom>? from, object state)
	{
		switch (from)
		{
			case null:
				return default;
		}

		var result = new TTo[from.Count];
		var idx = 0;

		foreach (var f in from)
		{
			result[idx++] = converter(f);
		}

		return result;
	}

	/// <summary>
	/// Initializes the.
	/// </summary>
	/// <param name="from">The from.</param>
	/// <param name="to">The to.</param>
	/// <param name="mappingConfig">The mapping config.</param>
	public void Initialize(Type from, Type to, MapConfigBaseImpl? mappingConfig)
	{
		var staticConverters = mappingConfig.GetStaticConvertersManager() ?? StaticConvertersManager.DefaultInstance;
		var staticConverterMethod = staticConverters.GetStaticConverter(Metadata<TFrom>.Type, Metadata<TTo>.Type);

		if (staticConverterMethod is not null)
		{
			converter = (Func<TFrom, TTo>)Delegate.CreateDelegate(
			  Metadata<Func<TFrom, TTo>>.Type,
			  default,
			  staticConverterMethod);
		}
		else
		{
			subMapper = Mapper.Default.GetMapperDescription(
			  Metadata<TFrom>.Type,
			  Metadata<TTo>.Type,
			  mappingConfig);

			converter = ConverterBySubmapper;
		}
	}

	/// <summary>
	/// Converters the by submapper.
	/// </summary>
	/// <param name="from">The from.</param>
	/// <returns>A TTo.</returns>
	private TTo ConverterBySubmapper(TFrom from)
	{
		return (TTo)subMapper.Mapper.Map(from);
	}
}