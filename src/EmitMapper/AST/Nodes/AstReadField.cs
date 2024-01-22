namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast read field.
/// </summary>
internal class AstReadField : IAstStackItem
{
	/// <summary>
	/// Field info
	/// </summary>
	public FieldInfo? FieldInfo;

	/// <summary>
	/// Source object
	/// </summary>
	public IAstRefOrAddr? SourceObject;

	/// <summary>
	///   Gets the item type.
	/// </summary>
	public Type ItemType => FieldInfo.FieldType;

	/// <inheritdoc />
	public virtual void Compile(CompilationContext context)
	{
		SourceObject.Compile(context);
		context.Emit(OpCodes.Ldfld, FieldInfo);
	}
}