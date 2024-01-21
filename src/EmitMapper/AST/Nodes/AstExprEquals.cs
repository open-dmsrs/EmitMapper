namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast expr equals.
/// </summary>
internal class AstExprEquals : IAstValue
{
	private readonly IAstValue leftValue;

	private readonly IAstValue rightValue;

	/// <summary>
	///   Initializes a new instance of the <see cref="AstExprEquals" /> class.
	/// </summary>
	/// <param name="leftValue">The left value.</param>
	/// <param name="rightValue">The right value.</param>
	public AstExprEquals(IAstValue leftValue, IAstValue rightValue)
	{
		this.leftValue = leftValue;
		this.rightValue = rightValue;
	}

	/// <summary>
	///   Gets the item type.
	/// </summary>
	public Type? ItemType => Metadata<int>.Type;

	/// <inheritdoc />
	public void Compile(CompilationContext context)
	{
		leftValue.Compile(context);
		rightValue.Compile(context);
		context.Emit(OpCodes.Ceq);
	}
}