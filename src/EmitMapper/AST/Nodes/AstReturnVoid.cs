using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

internal class AstReturnVoid : IAstNode
{
  public void Compile(CompilationContext context)
  {
    context.Emit(OpCodes.Ret);
  }
}