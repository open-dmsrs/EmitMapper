namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast write local.
/// </summary>
internal class AstWriteLocal : IAstNode
{
	public int LocalIndex;

	public Type LocalType;

	public IAstRefOrValue Value;

	/// <summary>
	///   Initializes a new instance of the <see cref="AstWriteLocal" /> class.
	/// </summary>
	public AstWriteLocal()
	{
	}

	/// <summary>
	///   Initializes a new instance of the <see cref="AstWriteLocal" /> class.
	/// </summary>
	/// <param name="loc">The loc.</param>
	/// <param name="value">The value.</param>
	public AstWriteLocal(LocalVariableInfo loc, IAstRefOrValue value)
	{
		LocalIndex = loc.LocalIndex;
		LocalType = loc.LocalType;
		Value = value;
	}

	/// <inheritdoc />
	public void Compile(CompilationContext context)
	{
		Value.Compile(context);
		CompilationHelper.PrepareValueOnStack(context, LocalType, Value.ItemType);
		context.Emit(OpCodes.Stloc, LocalIndex);
	}
}