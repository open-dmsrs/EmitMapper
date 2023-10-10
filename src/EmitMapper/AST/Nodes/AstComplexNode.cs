namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast complex node.
/// </summary>
internal class AstComplexNode : IAstNode
{
  public List<IAstNode> Nodes = new();

  /// <summary>
  /// </summary>
  /// <param name="context">The context.</param>
  public void Compile(CompilationContext context)
  {
    foreach (var node in Nodes)
      if (node != null)
        node.Compile(context);
  }
}