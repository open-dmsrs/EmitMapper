namespace EmitMapper.Conversion;

/// <summary>
///   The static converters manager.
/// </summary>
public class StaticConvertersManager
{
	private static readonly LazyConcurrentDictionary<MethodInfo, Func<object, object>?> ConvertersFunc = new();

	private static readonly object Locker = new();

	private static StaticConvertersManager? defaultInstance;

	private readonly LazyConcurrentDictionary<TypesPair, MethodInfo> typesMethods = new();

	private readonly List<Func<Type, Type, MethodInfo>> typesMethodsFunc = new();

	/// <summary>
	///   Gets the default instance.
	/// </summary>
	public static StaticConvertersManager DefaultInstance
	{
		get
		{
			switch (defaultInstance)
			{
				case null:
				{
					lock (Locker)
					{
						switch (defaultInstance)
						{
							case null:
								defaultInstance = new StaticConvertersManager();
								defaultInstance.AddConverterClass(Metadata.Convert);
								defaultInstance.AddConverterClass(Metadata<EmConvert>.Type);
								defaultInstance.AddConverterClass(Metadata<NullableConverter>.Type);
								defaultInstance.AddConverterFunc(EmConvert.GetConversionMethod);

								break;
						}
					}

					break;
				}
			}

			return defaultInstance;
		}
	}

	/// <summary>
	///   Adds the converter class.
	/// </summary>
	/// <param name="converterClass">The converter class.</param>
	public void AddConverterClass(Type? converterClass)
	{
		foreach (var m in converterClass.GetMethods(BindingFlags.Static | BindingFlags.Public))
		{
			var parameters = m.GetParameters();

			switch (parameters.Length)
			{
				case 1 when m.ReturnType != Metadata.Void:
					typesMethods.TryAdd(new TypesPair(parameters[0].ParameterType, m.ReturnType), m);

					break;
			}
		}
	}

	/// <summary>
	///   Adds the converter func.
	/// </summary>
	/// <param name="converterFunc">The converter func.</param>
	public void AddConverterFunc(Func<Type, Type, MethodInfo> converterFunc)
	{
		typesMethodsFunc.Add(converterFunc);
	}

	/// <summary>
	///   Gets the static converter.
	/// </summary>
	/// <param name="from">The from.</param>
	/// <param name="to">The to.</param>
	/// <returns>A MethodInfo.</returns>
	public MethodInfo? GetStaticConverter(Type from, Type to)
	{
		if (from is null || to is null)
		{
			return null;
		}

		foreach (var func in ((IEnumerable<Func<Type, Type, MethodInfo>>)typesMethodsFunc).Reverse())
		{
			var result = func(from, to);

			if (result is not null)
			{
				return result;
			}
		}

		typesMethods.TryGetValue(new TypesPair(from, to), out var res);

		return res;
	}

	/// <summary>
	///   Gets the static converter func.
	/// </summary>
	/// <param name="from">The from.</param>
	/// <param name="to">The to.</param>
	/// <returns><![CDATA[Func<object, object>]]></returns>
	public Func<object, object>? GetStaticConverterFunc(Type from, Type to)
	{
		var mi = GetStaticConverter(from, to);

		switch (mi)
		{
			case null:
				return null;
			default:
				return ConvertersFunc.GetOrAdd(mi, m => ((MethodInvokerFunc1)EmitInvoker.Methods.MethodInvoker.GetMethodInvoker(null, m)).CallFunc);
		}
	}
}