using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

namespace EmitMapper.MappingConfiguration.MappingOperations;

public class DestSrcReadOperation : IDestReadOperation, ISrcReadOperation
{
  public MemberDescriptor Destination { get; set; }

  public MemberDescriptor Source { get; set; }

  public ValueProcessor ValueProcessor { get; set; }

  public override string ToString()
  {
    return "DestSrcReadOperation. Source member:" + Source + " Target member:" + Destination;
  }
}