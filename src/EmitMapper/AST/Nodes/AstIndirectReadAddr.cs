using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

internal class AstIndirectReadAddr : AstIndirectRead, IAstAddr
{
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsValue(ItemType);
  }
}