using System;

namespace EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

public interface IReadWriteOperation : IDestWriteOperation, ISrcReadOperation
{
    Delegate NullSubstitutor { get; set; } // generic type: NullSubstitutor

    Delegate TargetConstructor { get; set; } // generic type: TargetConstructor

    Delegate Converter { get; set; }

    Delegate DestinationFilter { get; set; }

    Delegate SourceFilter { get; set; }
}