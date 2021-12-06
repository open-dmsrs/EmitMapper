namespace EmitMapper.MappingConfiguration.MappingOperations.Interfaces
{
    public interface IDestOperation : IMappingOperation
    {
        MemberDescriptor Destination { get; set; }
    }
}
