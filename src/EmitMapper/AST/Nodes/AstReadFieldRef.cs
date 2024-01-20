namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast read field ref.
/// </summary>
internal class AstReadFieldRef : AstReadField, IAstRef
{
	/// <inheritdoc />
	public override void Compile(CompilationContext context)
	{
		CompilationHelper.CheckIsRef(ItemType);
		base.Compile(context);
	}
}