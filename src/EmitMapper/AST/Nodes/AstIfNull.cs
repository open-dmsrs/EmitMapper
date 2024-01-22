namespace EmitMapper.AST.Nodes;

/// <summary>
///   Generates "value ?? ifNullValue" expression.
/// </summary>
internal class AstIfNull : IAstRefOrValue
{
	private readonly IAstRefOrValue ifNullValue;

	private readonly IAstRef value;

	/// <summary>
	///   Initializes a new instance of the <see cref="AstIfNull" /> class.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="ifNullValue">The if null value.</param>
	public AstIfNull(IAstRef value, IAstRefOrValue ifNullValue)
	{
		this.value = value;
		this.ifNullValue = ifNullValue;

		switch (this.value.ItemType.IsAssignableFrom(this.ifNullValue.ItemType))
		{
			case false:
				throw new EmitMapperException("Incorrect if null expression");
		}
	}

	/// <summary>
	///   Gets the item type.
	/// </summary>
	public Type ItemType => value.ItemType;

	/// <inheritdoc />
	public void Compile(CompilationContext context)
	{
		var ifNotNullLabel = context.IlGenerator.DefineLabel();
		value.Compile(context);
		context.Emit(OpCodes.Dup);
		context.Emit(OpCodes.Brtrue_S, ifNotNullLabel);
		context.Emit(OpCodes.Pop);
		ifNullValue.Compile(context);
		context.IlGenerator.MarkLabel(ifNotNullLabel);
	}
}