namespace EmitMapper.MappingConfiguration.MappingOperations
{
    public interface ISrcOperation : IMappingOperation
    {
        MemberDescriptor Source { get; set; }
    }
}
