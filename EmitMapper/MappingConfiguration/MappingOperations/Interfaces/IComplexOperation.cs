namespace EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

using System.Collections.Generic;

internal interface IComplexOperation : IMappingOperation
{
    List<IMappingOperation> Operations { get; set; }
}