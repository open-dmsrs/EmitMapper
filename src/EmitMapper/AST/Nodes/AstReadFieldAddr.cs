namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast read field addr.
/// </summary>
internal class AstReadFieldAddr : AstReadField, IAstAddr
{
	/// <inheritdoc />
	public override void Compile(CompilationContext context)
	{
		CompilationHelper.CheckIsValue(ItemType);
		SourceObject.Compile(context);
		context.Emit(OpCodes.Ldflda, FieldInfo);
	}
}