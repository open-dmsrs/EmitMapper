namespace EmitMapper.MappingConfiguration.MappingOperations
{
    public delegate void ValueProcessor(object srcValue, object dstValue, object state);
    public class DestSrcReadOperation : IDestReadOperation, ISrcReadOperation
    {
        public MemberDescriptor Destination { get; set; }
        public MemberDescriptor Source { get; set; }
        public ValueProcessor ValueProcessor { get; set; }

        public override string ToString()
        {
            return "DestSrcReadOperation. Source member:" + Source + " Target member:" + Destination.ToString();
        }
    }
}
