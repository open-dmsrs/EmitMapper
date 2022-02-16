namespace EmitMapper.AST.Nodes;

using System.Reflection.Emit;

using EmitMapper.AST.Interfaces;

internal class AstThrow : IAstNode
{
  public IAstRef Exception;

  public void Compile(CompilationContext context)
  {
    Exception.Compile(context);
    context.Emit(OpCodes.Throw);
  }
}