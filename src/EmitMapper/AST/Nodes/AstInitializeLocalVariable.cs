namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast initialize local variable.
/// </summary>
internal class AstInitializeLocalVariable : IAstNode
{
	public int LocalIndex;

	public Type? LocalType;

	/// <summary>
	///   Initializes a new instance of the <see cref="AstInitializeLocalVariable" /> class.
	/// </summary>
	public AstInitializeLocalVariable()
	{
	}

	/// <summary>
	///   Initializes a new instance of the <see cref="AstInitializeLocalVariable" /> class.
	/// </summary>
	/// <param name="loc">The loc.</param>
	public AstInitializeLocalVariable(LocalVariableInfo loc)
	{
		LocalType = loc.LocalType;
		LocalIndex = loc.LocalIndex;
	}

	/// <inheritdoc />
	public void Compile(CompilationContext context)
	{
		if (LocalType.IsValueType)
		{
			context.Emit(OpCodes.Ldloca, LocalIndex);
			context.Emit(OpCodes.Initobj, LocalType);
		}
	}
}