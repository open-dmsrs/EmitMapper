namespace EmitMapper.MappingConfiguration.MappingOperations;

/// <summary>
/// Value to write
/// </summary>
public struct ValueToWrite<T>
{
	/// <summary>
	/// Action
	/// </summary>
	public Actions Action;

	/// <summary>
	/// Value
	/// </summary>
	public T Value;

	/// <summary>
	/// The actions.
	/// </summary>
	public enum Actions
	{
		/// <summary>
		/// Write
		/// </summary>
		Write = 0,

		/// <summary>
		/// Skip
		/// </summary>
		Skip = 1
	}

	/// <summary>
	/// Returns the value.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>A ValueToWrite.</returns>
	public static ValueToWrite<T> ReturnValue(T? value)
	{
		return new ValueToWrite<T> { Action = Actions.Write, Value = value };
	}

	/// <summary>
	/// Skips the.
	/// </summary>
	/// <returns>A ValueToWrite.</returns>
	public static ValueToWrite<T> Skip()
	{
		return new ValueToWrite<T> { Action = Actions.Skip };
	}
}