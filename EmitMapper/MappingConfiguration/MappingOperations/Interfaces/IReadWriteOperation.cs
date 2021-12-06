using System;

namespace EmitMapper.MappingConfiguration.MappingOperations
{

    /* Unmerged change from project 'EmitMapper (netstandard2.1)'
    Before:
        interface IReadWriteOperation : IDestWriteOperation, ISrcReadOperation
    After:
        interface IReadWriteOperation : IDestWriteOperation, ISrcReadOperation
    */
    internal interface IReadWriteOperation : IDestWriteOperation, ISrcReadOperation
    {
        Delegate NullSubstitutor { get; set; } // generic type: NullSubstitutor
        Delegate TargetConstructor { get; set; } // generic type: TargetConstructor
        Delegate Converter { get; set; }
        Delegate DestinationFilter { get; set; }
        Delegate SourceFilter { get; set; }
    }
}
