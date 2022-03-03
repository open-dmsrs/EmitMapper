using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

internal class AstReadArrayItemRef : AstReadArrayItem, IAstRef
{
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsRef(ItemType);
    base.Compile(context);
  }
}