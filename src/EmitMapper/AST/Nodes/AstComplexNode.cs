namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast complex node.
/// </summary>
internal class AstComplexNode : IAstNode
{
	public List<IAstNode> Nodes = new();

	/// <inheritdoc />
	public void Compile(CompilationContext context)
	{
		foreach (var node in Nodes)
			node?.Compile(context);
	}
}