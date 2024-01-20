namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast read array item addr.
/// </summary>
internal class AstReadArrayItemAddr : AstReadArrayItem, IAstAddr
{
	/// <inheritdoc />
	public override void Compile(CompilationContext context)
	{
		CompilationHelper.CheckIsValue(ItemType);
		Array.Compile(context);
		context.Emit(OpCodes.Ldc_I4, Index);
		context.Emit(OpCodes.Ldelema, ItemType);
	}
}