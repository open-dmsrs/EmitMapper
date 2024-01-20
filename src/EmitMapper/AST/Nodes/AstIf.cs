namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast if.
/// </summary>
internal class AstIf : IAstNode
{
	public IAstValue Condition;

	public AstComplexNode FalseBranch;

	public AstComplexNode TrueBranch;

	/// <inheritdoc />
	public void Compile(CompilationContext context)
	{
		var elseLabel = context.ILGenerator.DefineLabel();
		var endIfLabel = context.ILGenerator.DefineLabel();

		Condition.Compile(context);
		context.Emit(OpCodes.Brfalse, elseLabel);

		TrueBranch?.Compile(context);

		if (FalseBranch != null)
		{
			context.Emit(OpCodes.Br, endIfLabel);
		}

		context.ILGenerator.MarkLabel(elseLabel);

		FalseBranch?.Compile(context);

		context.ILGenerator.MarkLabel(endIfLabel);
	}
}