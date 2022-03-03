using System;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

namespace EmitMapper.MappingConfiguration.MappingOperations;

public class DestWriteOperation : IDestWriteOperation
{
  public MemberDescriptor Destination { get; set; }

  public Delegate Getter { get; set; }

  public override string ToString()
  {
    return "DestWriteOperation. Target member:" + Destination;
  }
}