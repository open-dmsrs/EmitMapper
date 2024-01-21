namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast if.
/// </summary>
internal class AstIf : IAstNode
{
	public IAstValue? Condition;

	public AstComplexNode? FalseBranch;

	public AstComplexNode? TrueBranch;

	/// <inheritdoc />
	public void Compile(CompilationContext context)
	{
		var elseLabel = context.IlGenerator.DefineLabel();
		var endIfLabel = context.IlGenerator.DefineLabel();

		Condition.Compile(context);
		context.Emit(OpCodes.Brfalse, elseLabel);

		TrueBranch?.Compile(context);

		if (FalseBranch is not null)
		{
			context.Emit(OpCodes.Br, endIfLabel);
		}

		context.IlGenerator.MarkLabel(elseLabel);

		FalseBranch?.Compile(context);

		context.IlGenerator.MarkLabel(endIfLabel);
	}
}