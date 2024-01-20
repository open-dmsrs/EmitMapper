namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast throw.
/// </summary>
internal class AstThrow : IAstNode
{
	public IAstRef Exception;

	/// <inheritdoc />
	public void Compile(CompilationContext context)
	{
		Exception.Compile(context);
		context.Emit(OpCodes.Throw);
	}
}