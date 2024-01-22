namespace EmitMapper.Conversion;

/// <summary>
///   The native converter.
/// </summary>
internal class NativeConverter
{
	private static readonly Type?[] ConvertTypes =
	{
	Metadata<bool>.Type, Metadata<char>.Type, Metadata<sbyte>.Type, Metadata<byte>.Type, Metadata<short>.Type,
	Metadata<int>.Type, Metadata<long>.Type, Metadata<ushort>.Type, Metadata<uint>.Type, Metadata<ulong>.Type,
	Metadata<float>.Type, Metadata<double>.Type, Metadata<decimal>.Type, Metadata<DateTime>.Type,
	Metadata<string>.Type
  };
	private static readonly MethodInfo ObjectToStringMethod = Metadata<NativeConverter>.Type.GetMethod(
	  nameof(ObjectToString),
	  BindingFlags.NonPublic | BindingFlags.Static);
	private static readonly MethodInfo ChangeTypeMethod = Metadata<EmConvert>.Type.GetMethod(
	  nameof(EmConvert.ChangeType),
	  new[] { Metadata<object>.Type, Metadata<Type>.Type, Metadata<Type>.Type });

	private static readonly MethodInfo[] ConvertMethods =
	  Metadata.Convert.GetMethods(BindingFlags.Static | BindingFlags.Public);

	private static readonly LazyConcurrentDictionary<TypesPair, bool> IsNativeConvertionPossibleCache =
	  new(new TypesPair());

	/// <summary>
	///   Converts the <see cref="IAstRefOrValue" />.
	/// </summary>
	/// <param name="destinationType">The destination type.</param>
	/// <param name="sourceType">The source type.</param>
	/// <param name="sourceValue">The source value.</param>
	/// <returns>An IAstRefOrValue.</returns>
	public static IAstRefOrValue Convert(Type destinationType, Type sourceType, IAstRefOrValue sourceValue)
	{
		if (destinationType == sourceValue.ItemType)
		{
			return sourceValue;
		}

		if (destinationType == Metadata<string>.Type)
		{
			return new AstCallMethodRef(ObjectToStringMethod, null, new List<IAstStackItem> { sourceValue });
		}

		foreach (var m in ConvertMethods)
		{
			if (m.ReturnType == destinationType)
			{
				var parameters = m.GetParameters();

				switch (parameters.Length)
				{
					case 1 when parameters[0].ParameterType == sourceType:
						return AstBuildHelper.ICallMethod(m, null, new List<IAstStackItem> { sourceValue });
				}
			}
		}

		return AstBuildHelper.ICallMethod(
		  ChangeTypeMethod,
		  null,
		  new List<IAstStackItem>
		  {
		sourceValue, new AstTypeof { Type = sourceType }, new AstTypeof { Type = destinationType }
		  });
	}

	/// <summary>
	///   Are the native convertion possible.
	/// </summary>
	/// <param name="f">The f.</param>
	/// <param name="t">The t.</param>
	/// <returns>A bool.</returns>
	public static bool IsNativeConvertionPossible(Type f, Type t)
	{
		return IsNativeConvertionPossibleCache.GetOrAdd(
		  new TypesPair(f, t),
		  p =>
		  {
			  var from = p.SourceType;
			  var to = p.DestinationType;

			  if (from is null || to is null)
			  {
				  return false;
			  }

			  if (ConvertTypes.Contains(from) && ConvertTypes.Contains(to))
			  {
				  return true;
			  }

			  if (to == Metadata<string>.Type)
			  {
				  return true;
			  }

			  if (from == Metadata<string>.Type && to == Metadata<Guid>.Type)
			  {
				  return true;
			  }

			  switch (from.IsEnum)
			  {
				  case true when to.IsEnum:
				  case true when ConvertTypes.Contains(to):
					  return true;
			  }

			  switch (to.IsEnum)
			  {
				  case true when ConvertTypes.Contains(from):
					  return true;
			  }

			  if (ReflectionHelper.IsNullable(from))
			  {
				  return IsNativeConvertionPossible(from.GetUnderlyingTypeCache(), to);
			  }

			  if (ReflectionHelper.IsNullable(to))
			  {
				  return IsNativeConvertionPossible(from, to.GetUnderlyingTypeCache());
			  }

			  return false;
		  });
	}

	/// <summary>
	///   Objects the to string.
	/// </summary>
	/// <param name="obj">The obj.</param>
	/// <returns>A string.</returns>
	internal static string? ObjectToString(object? obj)
	{
		return obj?.ToString();
	}
}