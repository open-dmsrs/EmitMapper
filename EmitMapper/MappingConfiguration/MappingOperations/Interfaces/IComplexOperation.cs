using System.Collections.Generic;

namespace EmitMapper.MappingConfiguration.MappingOperations.Interfaces
{
    internal interface IComplexOperation : IMappingOperation
    {
        List<IMappingOperation> Operations { get; set; }
    }
}
