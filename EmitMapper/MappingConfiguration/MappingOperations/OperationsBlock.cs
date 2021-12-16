using System.Collections.Generic;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

namespace EmitMapper.MappingConfiguration.MappingOperations;

public class OperationsBlock : IComplexOperation
{
    public List<IMappingOperation> Operations { get; set; }
}