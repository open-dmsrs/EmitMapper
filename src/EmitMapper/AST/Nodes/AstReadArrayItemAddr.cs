using System.Reflection.Emit;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast read array item addr.
/// </summary>
internal class AstReadArrayItemAddr : AstReadArrayItem, IAstAddr
{
  /// <summary>
  /// </summary>
  /// <param name="context">The context.</param>
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsValue(ItemType);
    Array.Compile(context);
    context.Emit(OpCodes.Ldc_I4, Index);
    context.Emit(OpCodes.Ldelema, ItemType);
  }
}