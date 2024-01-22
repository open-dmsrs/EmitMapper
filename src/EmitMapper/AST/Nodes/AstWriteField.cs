namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast write field.
/// </summary>
internal class AstWriteField : IAstNode
{
	/// <summary>
	/// Field info
	/// </summary>
	public FieldInfo? FieldInfo;

	/// <summary>
	/// Target object
	/// </summary>
	public IAstRefOrAddr? TargetObject;

	/// <summary>
	/// Value
	/// </summary>
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