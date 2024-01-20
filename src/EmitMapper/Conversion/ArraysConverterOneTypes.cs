namespace EmitMapper.Conversion;

/// <summary>
///   The arrays converter one types.
/// </summary>
/// <typeparam name="T"></typeparam>
internal class ArraysConverterOneTypes<T>
{
	/// <summary>
	///   Converts the an array of TS.
	/// </summary>
	/// <param name="from">The from.</param>
	/// <param name="state">The state.</param>
	/// <returns>An array of TS</returns>
	public T[] Convert(ICollection<T> from, object state)
	{
		return from?.ToArray();
	}
}