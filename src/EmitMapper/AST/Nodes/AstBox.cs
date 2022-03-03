using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast box.
/// </summary>
internal class AstBox : IAstRef
{
  public IAstRefOrValue Value;

  /// <summary>
  ///   Gets the item type.
  /// </summary>
  public Type ItemType => Value.ItemType;

  /// <summary>
  /// </summary>
  /// <param name="context">The context.</param>
  public void Compile(CompilationContext context)
  {
    Value.Compile(context);

    if (Value.ItemType.IsValueType)
      context.Emit(OpCodes.Box, ItemType);
  }
}