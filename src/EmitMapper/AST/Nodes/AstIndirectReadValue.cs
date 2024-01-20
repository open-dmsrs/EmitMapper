namespace EmitMapper.AST.Nodes;

/// <summary>
/// The ast indirect read value.
/// </summary>
internal class AstIndirectReadValue : AstIndirectRead, IAstValue
{
	/// <inheritdoc/>
	/// <exception cref="Exception"></exception>
	public override void Compile(CompilationContext context)
	{
		CompilationHelper.CheckIsValue(ItemType);

		if (ItemType == Metadata<int>.Type)
		{
			context.Emit(OpCodes.Ldind_I4);
		}
		else
		{
			throw new NotSupportedException("Unsupported type");
		}
	}
}