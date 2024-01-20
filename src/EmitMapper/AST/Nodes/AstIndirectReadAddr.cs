namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast indirect read addr.
/// </summary>
internal class AstIndirectReadAddr : AstIndirectRead, IAstAddr
{
	/// <inheritdoc />
	public override void Compile(CompilationContext context)
	{
		CompilationHelper.CheckIsValue(ItemType);
	}
}