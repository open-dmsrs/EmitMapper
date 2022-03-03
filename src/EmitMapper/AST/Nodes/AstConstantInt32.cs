using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;
using EmitMapper.Utils;

namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast constant int32.
/// </summary>
internal class AstConstantInt32 : IAstValue
{
  public int Value;

  /// <summary>
  ///   Gets the item type.
  /// </summary>
  public Type ItemType => Metadata<int>.Type;

  /// <summary>
  /// </summary>
  /// <param name="context">The context.</param>
  public void Compile(CompilationContext context)
  {
    context.Emit(OpCodes.Ldc_I4, Value);
  }
}