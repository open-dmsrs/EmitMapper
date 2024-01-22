namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast if ternar.
/// </summary>
internal class AstIfTernar : IAstRefOrValue
{
	/// <summary>
	/// Condition
	/// </summary>
	public IAstRefOrValue Condition;

	/// <summary>
	/// False branch
	/// </summary>
	public IAstRefOrValue FalseBranch;

	/// <summary>
	/// True branch
	/// </summary>
	public IAstRefOrValue TrueBranch;

	/// <summary>
	///   Initializes a new instance of the <see cref="AstIfTernar" /> class.
	/// </summary>
	/// <param name="condition">The condition.</param>
	/// <param name="trueBranch">The true branch.</param>
	/// <param name="falseBranch">The false branch.</param>
	public AstIfTernar(IAstRefOrValue condition, IAstRefOrValue trueBranch, IAstRefOrValue falseBranch)
	{
		if (trueBranch.ItemType != falseBranch.ItemType)
		{
			throw new EmitMapperException("Types mismatch");
		}

		Condition = condition;
		TrueBranch = trueBranch;
		FalseBranch = falseBranch;
	}

	/// <summary>
	///   Gets the item type.
	/// </summary>
	public Type ItemType => TrueBranch.ItemType;

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