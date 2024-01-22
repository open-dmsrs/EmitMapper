namespace EmitMapper.AST.Nodes;

/// <summary>
/// The ast box.
/// </summary>
internal class AstBox : IAstRef
{
	/// <summary>
	/// Value
	/// </summary>
	public IAstRefOrValue? Value;

	/// <summary>
	/// Gets the item type.
	/// </summary>
	public Type ItemType => Value.ItemType;

	/// <inheritdoc/>
	public void Compile(CompilationContext context)
	{
		Value.Compile(context);

		if (Value.ItemType.IsValueType)
		{
			context.Emit(OpCodes.Box, ItemType);
		}
	}
}