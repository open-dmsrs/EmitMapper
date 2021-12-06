﻿using System;

namespace EmitMapper.MappingConfiguration.MappingOperations
{
    public interface IRootMappingOperation : IMappingOperation
    {
        Type From { get; set; }
        Type To { get; set; }
        Delegate NullSubstitutor { get; set; }
        Delegate TargetConstructor { get; set; }
        Delegate Converter { get; set; }
        bool ShallowCopy { get; set; }
        Delegate ValuesPostProcessor { get; set; }
        Delegate DestinationFilter { get; set; }
        Delegate SourceFilter { get; set; }
    }
}
