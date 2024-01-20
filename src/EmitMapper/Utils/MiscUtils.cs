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
	/// <param name="delim">The delim.</param>
	/// <returns>A string.</returns>
	public static string ToCsv<T>(this IEnumerable<T> collection, string delim)
	{
		if (collection is null)
		{
			return string.Empty;
		}

		return string.Join(delim, collection);
	}
}