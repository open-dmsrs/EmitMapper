using System;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

namespace EmitMapper.MappingConfiguration.MappingOperations;

/// <summary>
///   The read write simple.
/// </summary>
public class ReadWriteSimple : IReadWriteOperation
{
  /// <summary>
  ///   Gets or Sets the converter.
  /// </summary>
  public Delegate Converter { get; set; }

  /// <summary>
  ///   Gets or Sets the destination.
  /// </summary>
  public MemberDescriptor Destination { get; set; }

  /// <summary>
  ///   Gets or Sets the destination filter.
  /// </summary>
  public Delegate DestinationFilter { get; set; }

  /// <summary>
  ///   Gets or Sets the null substitutor.
  /// </summary>
  public Delegate NullSubstitutor { get; set; }

  /// <summary>
  ///   Gets or Sets a value indicating whether shallow copy.
  /// </summary>
  public bool ShallowCopy { get; set; }

  /// <summary>
  ///   Gets or Sets the source.
  /// </summary>
  public MemberDescriptor Source { get; set; }

  /// <summary>
  ///   Gets or Sets the source filter.
  /// </summary>
  public Delegate SourceFilter { get; set; }

  /// <summary>
  ///   Gets or Sets the target constructor.
  /// </summary>
  public Delegate TargetConstructor { get; set; }

  /// <summary>
  ///   Tos the string.
  /// </summary>
  /// <returns>A string.</returns>
  public override string ToString()
  {
    return "ReadWriteSimple. Source member:" + Source + " Target member:" + Destination;
  }
}