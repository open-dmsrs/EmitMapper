using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

namespace EmitMapper.MappingConfiguration.MappingOperations;

public delegate void ValueProcessor(object srcValue, object dstValue, object state);

public class DestSrcReadOperation : IDestReadOperation, ISrcReadOperation
{
  public ValueProcessor ValueProcessor { get; set; }

  public MemberDescriptor Destination { get; set; }

  public MemberDescriptor Source { get; set; }

  public override string ToString()
  {
    return "DestSrcReadOperation. Source member:" + Source + " Target member:" + Destination;
  }
}