using System;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

namespace EmitMapper.MappingConfiguration.MappingOperations;

public delegate ValueToWrite<T> ValueGetter<T>(object value, object state);

public struct ValueToWrite<T>
{
  public enum Actions
  {
    Write = 0,

    Skip = 1
  }

  public T Value;

  public Actions Action;

  public static ValueToWrite<T> ReturnValue(T value)
  {
    return new ValueToWrite<T> { Action = Actions.Write, Value = value };
  }

  public static ValueToWrite<T> Skip()
  {
    return new ValueToWrite<T> { Action = Actions.Skip };
  }
}

public class DestWriteOperation : IDestWriteOperation
{
  public Delegate Getter { get; set; }

  public MemberDescriptor Destination { get; set; }

  public override string ToString()
  {
    return "DestWriteOperation. Target member:" + Destination;
  }
}