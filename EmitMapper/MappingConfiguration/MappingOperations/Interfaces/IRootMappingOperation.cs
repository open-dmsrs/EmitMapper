using System;

namespace EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

public interface IRootMappingOperation : IMappingOperation
{
  Delegate Converter { get; set; }

  Delegate DestinationFilter { get; set; }

  Type From { get; set; }

  Delegate NullSubstitutor { get; set; }

  bool ShallowCopy { get; set; }

  Delegate SourceFilter { get; set; }

  Delegate TargetConstructor { get; set; }

  Type To { get; set; }

  Delegate ValuesPostProcessor { get; set; }
}