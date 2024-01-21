namespace EmitMapper.Utils;

/// <summary>
///   The misc utils.
/// </summary>
internal static class MiscUtils
{
	/// <summary>
	///   Tos the csv.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="collection">The collection.</param>
	/// <param name="separator">The separator.</param>
	/// <returns>A string.</returns>
	public static string ToCsv<T>(this IEnumerable<T>? collection, string separator)
	{
		if (collection is null)
		{
			return string.Empty;
		}

		return string.Join(separator, collection);
	}
}