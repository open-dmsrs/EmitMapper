using System.Collections.Generic;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;

namespace EmitMapper.MappingConfiguration.MappingOperations;

/// <summary>
///   The operations block.
/// </summary>
public class OperationsBlock : IComplexOperation
{
  /// <summary>
  ///   Gets or Sets the operations.
  /// </summary>
  public List<IMappingOperation> Operations { get; set; }
}