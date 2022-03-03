using System;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast indirect read.
/// </summary>
internal abstract class AstIndirectRead : IAstStackItem
{
  /// <summary>
  ///   Gets or Sets the item type.
  /// </summary>
  public Type ItemType { get; set; }

  /// <summary>
  /// </summary>
  /// <param name="context">The context.</param>
  public abstract void Compile(CompilationContext context);
}