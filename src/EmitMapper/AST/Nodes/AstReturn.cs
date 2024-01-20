namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast return.
/// </summary>
internal class AstReturn : IAstAddr
{
	public Type ReturnType;

	public IAstRefOrValue ReturnValue;

	/// <summary>
	///   Gets the item type.
	/// </summary>
	public Type ItemType => ReturnType;

	/// <inheritdoc />
	public void Compile(CompilationContext context)
	{
		ReturnValue.Compile(context);
		CompilationHelper.PrepareValueOnStack(context, ReturnType, ReturnValue.ItemType);
		context.Emit(OpCodes.Ret);
	}
}