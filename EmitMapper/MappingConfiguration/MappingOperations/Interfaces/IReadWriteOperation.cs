namespace EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

using System;

public interface IReadWriteOperation : IDestWriteOperation, ISrcReadOperation
{
  Delegate Converter { get; set; }

  Delegate DestinationFilter { get; set; }

  Delegate NullSubstitutor { get; set; } // generic type: NullSubstitutor

  Delegate SourceFilter { get; set; }

  Delegate TargetConstructor { get; set; } // generic type: TargetConstructor
}
//public class ReadWriteOperation : IReadWriteOperation
//{
//  public Delegate NullSubstitutor { get; set; }

//  public Type NullSubstitutorType { get; }
//  public Delegate TargetConstructor { get; set; }
//  public Type TypeTargetConstructor { get; }
//  public Delegate Converter { get; set; }
//  public Type TypeConverter { get; set; }
//  public Delegate DestinationFilter { get; set; }
//  public Type TypeDestinationFilter { get; }

//  public Delegate SourceFilter { get; set; }
//  public Type TypeSourceFilter { get; }

//  public MemberDescriptor Destination { get; set; }
//  public MemberDescriptor Source { get; set; }
//}