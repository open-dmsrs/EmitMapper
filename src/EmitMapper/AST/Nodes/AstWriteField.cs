namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast write field.
/// </summary>
internal class AstWriteField : IAstNode
{
	public FieldInfo? FieldInfo;

	public IAstRefOrAddr? TargetObject;

	public IAstRefOrValue? Value;

	/// <inheritdoc />
	public void Compile(CompilationContext context)
	{
		TargetObject.Compile(context);
		Value.Compile(context);
		CompilationHelper.PrepareValueOnStack(context, FieldInfo.FieldType, Value.ItemType);
		context.Emit(OpCodes.Stfld, FieldInfo);
	}
}