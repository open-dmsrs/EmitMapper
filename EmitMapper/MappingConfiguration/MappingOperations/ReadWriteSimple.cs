namespace EmitMapper.MappingConfiguration.MappingOperations;

using System;

using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

public class ReadWriteSimple : IReadWriteOperation
{
  public Delegate Converter { get; set; }

  public MemberDescriptor Destination { get; set; }

  public Delegate DestinationFilter { get; set; }

  public Delegate NullSubstitutor { get; set; }

  public bool ShallowCopy { get; set; }

  public MemberDescriptor Source { get; set; }

  public Delegate SourceFilter { get; set; }

  public Delegate TargetConstructor { get; set; }

  public override string ToString()
  {
    return "ReadWriteSimple. Source member:" + Source + " Target member:" + Destination;
  }
}