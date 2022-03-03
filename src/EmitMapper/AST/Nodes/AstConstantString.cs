using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;
using EmitMapper.Utils;

namespace EmitMapper.AST.Nodes;
/// <summary>
/// The ast constant string.
/// </summary>

internal class AstConstantString : IAstRef
{
  public string Str;

  /// <summary>
  /// Gets the item type.
  /// </summary>
  public Type ItemType => Metadata<string>.Type;

  /// <summary>
  /// 
  /// </summary>
  /// <param name="context">The context.</param>
  public void Compile(CompilationContext context)
  {
    context.Emit(OpCodes.Ldstr, Str);
  }
}