namespace EmitMapper.MappingConfiguration.MappingOperations;

public struct ValueToWrite<T>
{
  public Actions Action;

  public T Value;

  /// <summary>
  ///   The actions.
  /// </summary>
  public enum Actions
  {
    Write = 0,

    Skip = 1
  }

  /// <summary>
  /// Returns the value.
  /// </summary>
  /// <param name="value">The value.</param>
  /// <returns>A ValueToWrite.</returns>
  public static ValueToWrite<T> ReturnValue(T value)
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