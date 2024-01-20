namespace EmitMapper.Conversion;

/// <summary>
/// The e m convert.
/// </summary>
public class EMConvert
{
	/// <summary>
	/// Changes the type.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="conversionType">The conversion type.</param>
	/// <returns>An object.</returns>
	public static object? ChangeType(object value, Type conversionType)
	{
		if (value is null)
		{
			return null;
		}

		return ChangeType(value, value.GetType(), conversionType);
	}

	/// <summary>
	/// Changes the type.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="typeFrom">The type from.</param>
	/// <param name="typeTo">The type to.</param>
	/// <returns>An object.</returns>
	public static object? ChangeType(object value, Type typeFrom, Type typeTo)
	{
		if (value is null)
		{
			return null;
		}

		if (typeTo.IsEnum)
		{
			return ConvertToEnum(value, typeFrom, typeTo);
		}

		if (typeFrom.IsEnum)
		{
			if (typeTo == Metadata<string>.Type)
			{
				return value.ToString();
			}

			return ChangeType(
			  Convert.ChangeType(value, Enum.GetUnderlyingType(typeFrom)),
			  Enum.GetUnderlyingType(typeFrom),
			  typeTo);
		}

		if (typeTo == Metadata<Guid>.Type)
		{
			var r = new Guid(value.ToString()!);

			return r == Guid.Empty ? new Guid() : r;
		}

		var isFromNullable = ReflectionHelper.IsNullable(typeFrom);
		var isToNullable = ReflectionHelper.IsNullable(typeTo);

		if (isFromNullable && !isToNullable)
		{
			return ChangeType(value, typeFrom.GetUnderlyingTypeCache(), typeTo);
		}

		if (isToNullable)
		{
			var ut = typeTo.GetUnderlyingTypeCache();

			if (ut.IsEnum)
			{
				return ConvertToEnum(value, typeFrom, ut);
			}

			return ChangeType(value, typeFrom, ut);
		}

		return Convert.ChangeType(value, typeTo);
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
	public static MethodInfo? GetConversionMethod(Type from, Type to)
	{
		if (from is null || to is null)
		{
			return null;
		}

		if (to == Metadata<string>.Type)
		{
			return Metadata<EMConvert>.Type.GetMethod(nameof(ObjectToString), BindingFlags.Static | BindingFlags.Public);
		}

		if (to.IsEnum)
		{
			return Metadata<EMConvert>.Type.GetMethod(nameof(ToEnum), BindingFlags.Static | BindingFlags.Public)
			  ?.MakeGenericMethod(to, Enum.GetUnderlyingType(to));
		}

		if (IsComplexConvert(from) || IsComplexConvert(to))
		{
			return Metadata<EMConvert>.Type.GetMethod(nameof(ChangeTypeGeneric), BindingFlags.Static | BindingFlags.Public)
			  ?.MakeGenericMethod(from, to);
		}

		return null;
	}

	/// <summary>
	/// Objects the to string.
	/// </summary>
	/// <param name="obj">The obj.</param>
	/// <returns>A string.</returns>
	public static string? ObjectToString(object obj)
	{
		if (obj is null)
		{
			return null;
		}

		return obj.ToString();
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
		if (obj is string)
		{
			var str = obj.ToString();

			return (TEnum)Enum.Parse(Metadata<TEnum>.Type, str);
		}

		return (TEnum)Convert.ChangeType(obj, Metadata<TUnder>.Type);
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
		if (!typeFrom.IsEnum)
		{
			if (typeFrom == Metadata<string>.Type)
			{
				return Enum.Parse(typeTo, value.ToString());
			}
		}

		return Enum.ToObject(typeTo, Convert.ChangeType(value, Enum.GetUnderlyingType(typeTo)));
	}

	/// <summary>
	/// Are the complex convert.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>A bool.</returns>
	private static bool IsComplexConvert(Type type)
	{
		if (type.IsEnum)
		{
			return true;
		}

		if (ReflectionHelper.IsNullable(type) && type.GetUnderlyingTypeCache().IsEnum)
		{
			return true;
		}

		return false;
	}
}