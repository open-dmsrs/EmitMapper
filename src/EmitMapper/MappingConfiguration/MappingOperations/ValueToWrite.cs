namespace EmitMapper.MappingConfiguration.MappingOperations;

public struct ValueToWrite<T>
{
  public Actions Action;

  public T Value;

  /// <summary>
  /// The actions.
  /// </summary>
  public enum Actions
  {
    Write = 0,

    Skip = 1
  }

  public static ValueToWrite<T> ReturnValue(T value)
  {
    return new ValueToWrite<T> { Action = Actions.Write, Value = value };
  }

  public static ValueToWrite<T> Skip()
  {
    return new ValueToWrite<T> { Action = Actions.Skip };
  }
}