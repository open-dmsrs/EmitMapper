namespace EmitMapper.Utils;

/// <summary>
///   The primitive helper.
/// </summary>
public static class PrimitiveHelper
{
	/// <summary>
	///   Checks the is derived from.
	/// </summary>
	/// <param name="types">The types.</param>
	/// <param name="baseTypes">The base types.</param>
	public static void CheckIsDerivedFrom(this in TypesPair types, in TypesPair baseTypes)
	{
		types.SourceType.CheckIsDerivedFrom(baseTypes.SourceType);
		types.DestinationType.CheckIsDerivedFrom(baseTypes.DestinationType);
	}

	//public static IEnumerable<T> Concat<T>(this IReadOnlyCollection<T> collection, IReadOnlyCollection<T> otherCollection)
	//{
	//  otherCollection ??= Array.Empty<T>();
	//  if (collection.Count == 0) return otherCollection;
	//  return otherCollection.Count == 0 ? collection : collection.Concat(otherCollection);
	//}

	/// <summary>
	///   Gets the or default.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	/// <param name="dictionary">The dictionary.</param>
	/// <param name="key">The key.</param>
	/// <returns>A <typeparamref name="TValue"></typeparamref></returns>
	public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
	{
		dictionary.TryGetValue(key, out var value);

		return value;
	}

	/// <summary>
	///   Are the enum to enum.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <returns>A bool.</returns>
	public static bool IsEnumToEnum(this in TypesPair context)
	{
		return context.SourceType.IsEnum && context.DestinationType.IsEnum;
	}

	/// <summary>
	///   Are the enum to underlying type.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <returns>A bool.</returns>
	public static bool IsEnumToUnderlyingType(this in TypesPair context)
	{
		return context.SourceType.IsEnum
			   && context.DestinationType.IsAssignableFrom(Enum.GetUnderlyingType(context.SourceType));
	}

	/// <summary>
	///   Are the underlying type to enum.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <returns>A bool.</returns>
	public static bool IsUnderlyingTypeToEnum(this in TypesPair context)
	{
		return context.DestinationType.IsEnum
			   && context.SourceType.IsAssignableFrom(Enum.GetUnderlyingType(context.DestinationType));
	}

	/// <summary>
	///   Nulls the check.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="source">The source.</param>
	/// <returns><![CDATA[IReadOnlyCollection<T>]]></returns>
	public static IReadOnlyCollection<T> NullCheck<T>(this IReadOnlyCollection<T> source)
	{
		return source ?? Array.Empty<T>();
	}
}