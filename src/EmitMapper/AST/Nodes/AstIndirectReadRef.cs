using System.Reflection.Emit;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast indirect read ref.
/// </summary>
internal class AstIndirectReadRef : AstIndirectRead, IAstRef
{
  /// <summary>
  /// </summary>
  /// <param name="context">The context.</param>
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsRef(ItemType);
    context.Emit(OpCodes.Ldind_Ref, ItemType);
  }
}