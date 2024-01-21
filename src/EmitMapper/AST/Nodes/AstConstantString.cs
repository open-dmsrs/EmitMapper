namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast constant string.
/// </summary>
internal class AstConstantString : IAstRef
{
	public string? Str;

	/// <summary>
	///   Gets the item type.
	/// </summary>
	public Type? ItemType => Metadata<string>.Type;

	/// <inheritdoc />
	public void Compile(CompilationContext context)
	{
		context.Emit(OpCodes.Ldstr, Str);
	}
}