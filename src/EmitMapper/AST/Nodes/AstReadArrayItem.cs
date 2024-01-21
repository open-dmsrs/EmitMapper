namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast read array item.
/// </summary>
internal class AstReadArrayItem : IAstStackItem
{
	public IAstRef? Array;

	public int Index;

	/// <summary>
	///   Gets the item type.
	/// </summary>
	public Type? ItemType => Array.ItemType.GetElementType();

	/// <inheritdoc />
	public virtual void Compile(CompilationContext context)
	{
		Array.Compile(context);
		context.Emit(OpCodes.Ldc_I4, Index);
		context.Emit(OpCodes.Ldelem, ItemType);
	}
}