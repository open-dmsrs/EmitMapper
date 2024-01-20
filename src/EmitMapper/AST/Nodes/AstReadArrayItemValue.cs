namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast read array item value.
/// </summary>
internal class AstReadArrayItemValue : AstReadArrayItem, IAstValue
{
	/// <inheritdoc />
	public override void Compile(CompilationContext context)
	{
		CompilationHelper.CheckIsValue(ItemType);
		base.Compile(context);
	}
}