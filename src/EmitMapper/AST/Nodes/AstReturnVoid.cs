using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;
/// <summary>
/// The ast return void.
/// </summary>

internal class AstReturnVoid : IAstNode
{
  /// <summary>
  /// 
  /// </summary>
  /// <param name="context">The context.</param>
  public void Compile(CompilationContext context)
  {
    context.Emit(OpCodes.Ret);
  }
}