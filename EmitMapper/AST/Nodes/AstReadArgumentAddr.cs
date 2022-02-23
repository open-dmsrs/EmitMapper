using System.Reflection.Emit;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

internal class AstReadArgumentAddr : AstReadArgument, IAstAddr
{
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsValue(ItemType);
    context.Emit(OpCodes.Ldarga, ArgumentIndex);
  }
}