using System.Reflection.Emit;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

internal class AstIndirectReadRef : AstIndirectRead, IAstRef
{
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsRef(ItemType);
    context.Emit(OpCodes.Ldind_Ref, ItemType);
  }
}