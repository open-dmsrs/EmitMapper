namespace EmitMapper.AST.Nodes;

/// <summary>
///   Generates "value ?? ifNullValue" expression.
/// </summary>
internal class AstIfNull : IAstRefOrValue
{
	private readonly IAstRefOrValue _ifNullValue;

	private readonly IAstRef _value;

	/// <summary>
	///   Initializes a new instance of the <see cref="AstIfNull" /> class.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="ifNullValue">The if null value.</param>
	public AstIfNull(IAstRef value, IAstRefOrValue ifNullValue)
	{
		this._value = value;
		this._ifNullValue = ifNullValue;

		if (!this._value.ItemType.IsAssignableFrom(this._ifNullValue.ItemType))
		{
			throw new EmitMapperException("Incorrect if null expression");
		}
	}

	/// <summary>
	///   Gets the item type.
	/// </summary>
	public Type ItemType => _value.ItemType;

	/// <inheritdoc />
	public void Compile(CompilationContext context)
	{
		var ifNotNullLabel = context.IlGenerator.DefineLabel();
		_value.Compile(context);
		context.Emit(OpCodes.Dup);
		context.Emit(OpCodes.Brtrue_S, ifNotNullLabel);
		context.Emit(OpCodes.Pop);
		_ifNullValue.Compile(context);
		context.IlGenerator.MarkLabel(ifNotNullLabel);
	}
}