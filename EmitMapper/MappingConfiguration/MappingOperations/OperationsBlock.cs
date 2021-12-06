using System.Collections.Generic;

namespace EmitMapper.MappingConfiguration.MappingOperations
{
    public class OperationsBlock : IComplexOperation
    {
        public List<IMappingOperation> Operations { get; set; }
    }
}
