namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast read argument.
/// </summary>
internal class AstReadArgument : IAstStackItem
{
	public int ArgumentIndex;

	public Type? ArgumentType;

	/// <summary>
	///   Gets the item type.
	/// </summary>
	public Type? ItemType => ArgumentType;

	/// <inheritdoc />
	public virtual void Compile(CompilationContext context)
	{
		switch (ArgumentIndex)
		{
			case 0:
				context.Emit(OpCodes.Ldarg_0);

				break;
			case 1:
				context.Emit(OpCodes.Ldarg_1);

				break;
			case 2:
				context.Emit(OpCodes.Ldarg_2);

				break;
			case 3:
				context.Emit(OpCodes.Ldarg_3);

				break;
			default:
				context.Emit(OpCodes.Ldarg, ArgumentIndex);

				break;
		}
	}
}