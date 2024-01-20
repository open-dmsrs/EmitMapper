namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast read local ref.
/// </summary>
internal class AstReadLocalRef : AstReadLocal, IAstRef
{
	/// <inheritdoc />
	public override void Compile(CompilationContext context)
	{
		CompilationHelper.CheckIsRef(ItemType);
		base.Compile(context);
	}
}