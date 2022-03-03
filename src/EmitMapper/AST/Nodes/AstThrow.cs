using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast throw.
/// </summary>
internal class AstThrow : IAstNode
{
  public IAstRef Exception;

  /// <summary>
  /// </summary>
  /// <param name="context">The context.</param>
  public void Compile(CompilationContext context)
  {
    Exception.Compile(context);
    context.Emit(OpCodes.Throw);
  }
}