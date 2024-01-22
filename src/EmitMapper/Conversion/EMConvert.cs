namespace EmitMapper.Conversion;

/// <summary>
/// The em convert.
/// </summary>
public class EmConvert
{
	/// <summary>
	/// Changes the type.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="conversionType">The conversion type.</param>
	/// <returns>An object.</returns>
	public static object? ChangeType(object? value, Type conversionType)
	{
		switch (value)
		{
			case null:
				return null;
			default:
				return ChangeType(value, value.GetType(), conversionType);
		}
	}

	/// <summary>
	/// Changes the type.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="typeFrom">The type from.</param>
	/// <param name="typeTo">The type to.</param>
	/// <returns>An object.</returns>
	public static object? ChangeType(object? value, Type typeFrom, Type typeTo)
	{
		switch (value)
		{
			case null:
				return null;
		}

		switch (typeTo.IsEnum)
		{
			case true:
				return ConvertToEnum(value, typeFrom, typeTo);
		}

		switch (typeFrom.IsEnum)
		{
			case true when typeTo == Metadata<string>.Type:
				return value.ToString();
			case true:
				return ChangeType(Convert.ChangeType(value, Enum.GetUnderlyingType(typeFrom), CultureInfo.InvariantCulture), Enum.GetUnderlyingType(typeFrom), typeTo);
		}

		if (typeTo == Metadata<Guid>.Type)
		{
			var r = new Guid(value.ToString()!);

			return r == Guid.Empty ? Guid.Empty : r;
		}

		var isFromNullable = ReflectionHelper.IsNullable(typeFrom);
		var isToNullable = ReflectionHelper.IsNullable(typeTo);

		switch (isFromNullable)
		{
			case true when !isToNullable:
				return ChangeType(value, typeFrom.GetUnderlyingTypeCache(), typeTo);
		}

		switch (isToNullable)
		{
			case true:
			{
				var ut = typeTo.GetUnderlyingTypeCache();

				switch (ut.IsEnum)
				{
					case true:
						return ConvertToEnum(value, typeFrom, ut);
					default:
						return ChangeType(value, typeFrom, ut);
				}
			}
			default:
				return Convert.ChangeType(value, typeTo, CultureInfo.InvariantCulture);
		}
	}

	/// <summary>
	/// Changes the type generic.
	/// </summary>
	/// <typeparam name="TFrom"></typeparam>
	/// <typeparam name="TTo"></typeparam>
	/// <param name="value">The value.</param>
	/// <returns>An object.</returns>
	public static object ChangeTypeGeneric<TFrom, TTo>(object value)
	{
		return ChangeType(value, Metadata<TFrom>.Type, Metadata<TTo>.Type);
	}

	/// <summary>
	/// Gets the conversion method.
	/// </summary>
	/// <param name="from">The from.</param>
	/// <param name="to">The to.</param>
	/// <returns>A MethodInfo.</returns>
	public static MethodInfo? GetConversionMethod(Type? from, Type? to)
	{
		if (from is null || to is null)
		{
			return null;
		}

		if (to == Metadata<string>.Type)
		{
			return Metadata<EmConvert>.Type.GetMethod(nameof(ObjectToString), BindingFlags.Static | BindingFlags.Public);
		}

		switch (to.IsEnum)
		{
			case true:
				return Metadata<EmConvert>.Type.GetMethod(nameof(ToEnum), BindingFlags.Static | BindingFlags.Public)
					?.MakeGenericMethod(to, Enum.GetUnderlyingType(to));
		}

		if (IsComplexConvert(from) || IsComplexConvert(to))
		{
			return Metadata<EmConvert>.Type.GetMethod(nameof(ChangeTypeGeneric), BindingFlags.Static | BindingFlags.Public)
			  ?.MakeGenericMethod(from, to);
		}

		return null;
	}

	/// <summary>
	/// Objects the to string.
	/// </summary>
	/// <param name="obj">The obj.</param>
	/// <returns>A string.</returns>
	public static string? ObjectToString(object? obj)
	{
		return obj?.ToString();
	}

	/// <summary>
	/// Strings the to guid.
	/// </summary>
	/// <param name="str">The str.</param>
	/// <returns>A Guid.</returns>
	public static Guid StringToGuid(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return Guid.Empty;
		}

		return new Guid(str);
	}

	/// <summary>
	/// Tos the enum.
	/// </summary>
	/// <typeparam name="TEnum"></typeparam>
	/// <typeparam name="TUnder"></typeparam>
	/// <param name="obj">The obj.</param>
	/// <returns>A <typeparamref name="TEnum"></typeparamref></returns>
	public static TEnum ToEnum<TEnum, TUnder>(object obj)
	{
		switch (obj)
		{
			case string:
			{
				var str = obj.ToString();

				return (TEnum)Enum.Parse(Metadata<TEnum>.Type, str ?? string.Empty);
			}
			default:
				return (TEnum)Convert.ChangeType(obj, Metadata<TUnder>.Type, CultureInfo.InvariantCulture);
		}
	}

	/// <summary>
	/// Converts the to enum.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="typeFrom">The type from.</param>
	/// <param name="typeTo">The type to.</param>
	/// <returns>An object.</returns>
	private static object ConvertToEnum(object value, Type typeFrom, Type typeTo)
	{
		switch (typeFrom.IsEnum)
		{
			case true:
				return Enum.ToObject(typeTo, Convert.ChangeType(value, Enum.GetUnderlyingType(typeTo), CultureInfo.InvariantCulture));
			default:
				return typeFrom == Metadata<string>.Type ? Enum.Parse(typeTo, value.ToString() ?? string.Empty) : Enum.ToObject(typeTo, Convert.ChangeType(value, Enum.GetUnderlyingType(typeTo), CultureInfo.InvariantCulture));
		}
	}

	/// <summary>
	/// Are the complex convert.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>A bool.</returns>
	private static bool IsComplexConvert(Type type)
	{
		switch (type)
		{
			case { IsEnum: true }:
				return true;
		}

		if (ReflectionHelper.IsNullable(type) && type.GetUnderlyingTypeCache().IsEnum)
		{
			return true;
		}

		return false;
	}
}