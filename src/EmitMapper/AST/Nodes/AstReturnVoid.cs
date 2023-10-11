namespace EmitMapper.AST.Nodes;

/// <summary>
/// The ast return void.
/// </summary>
internal class AstReturnVoid : IAstNode
{
  /// <inheritdoc/>
  public void Compile(CompilationContext context)
  {
    context.Emit(OpCodes.Ret);
  }
}