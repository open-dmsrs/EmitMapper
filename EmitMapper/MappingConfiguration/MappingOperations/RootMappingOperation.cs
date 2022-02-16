namespace EmitMapper.MappingConfiguration.MappingOperations;

using System;

using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

public class RootMappingOperation : IRootMappingOperation
{
  public RootMappingOperation(Type from, Type to)
  {
    From = from;
    To = to;
  }

  public Delegate Converter { get; set; }

  public Delegate DestinationFilter { get; set; }

  public Type From { get; set; }

  public Delegate NullSubstitutor { get; set; }

  public bool ShallowCopy { get; set; }

  public Delegate SourceFilter { get; set; }

  public Delegate TargetConstructor { get; set; }

  public Type To { get; set; }

  public Delegate ValuesPostProcessor { get; set; }
}