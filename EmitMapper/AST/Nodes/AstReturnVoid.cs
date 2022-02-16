namespace EmitMapper.AST.Nodes;

using System.Reflection.Emit;

using EmitMapper.AST.Interfaces;

internal class AstReturnVoid : IAstNode
{
  public void Compile(CompilationContext context)
  {
    context.Emit(OpCodes.Ret);
  }
}