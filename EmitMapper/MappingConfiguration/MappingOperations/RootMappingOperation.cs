using System;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

namespace EmitMapper.MappingConfiguration.MappingOperations;

public class RootMappingOperation : IRootMappingOperation
{
  public RootMappingOperation(Type from, Type to)
  {
    From = from;
    To = to;
  }

  #region IRootMappingOperation Members

  public Type From { get; set; }

  public Type To { get; set; }

  public Delegate NullSubstitutor { get; set; }

  public Delegate TargetConstructor { get; set; }

  public Delegate Converter { get; set; }

  public bool ShallowCopy { get; set; }

  public Delegate ValuesPostProcessor { get; set; }

  public Delegate DestinationFilter { get; set; }

  public Delegate SourceFilter { get; set; }

  #endregion
}