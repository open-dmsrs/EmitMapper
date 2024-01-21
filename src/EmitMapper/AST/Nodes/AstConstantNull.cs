namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast constant null.
/// </summary>
internal class AstConstantNull : IAstRefOrValue
{
	/// <summary>
	///   Gets the item type.
	/// </summary>
	public Type? ItemType => Metadata<object>.Type;

	/// <inheritdoc />
	public void Compile(CompilationContext context)
	{
		context.Emit(OpCodes.Ldnull);
	}
}