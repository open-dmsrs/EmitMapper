namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast expr is null.
/// </summary>
internal class AstExprIsNull : IAstValue
{
	private readonly IAstRefOrValue value;

	/// <summary>
	///   Initializes a new instance of the <see cref="AstExprIsNull" /> class.
	/// </summary>
	/// <param name="value">The value.</param>
	public AstExprIsNull(IAstRefOrValue value)
	{
		this.value = value;
	}

	/// <summary>
	///   Gets the item type.
	/// </summary>
	public Type ItemType => Metadata<int>.Type;

	/// <inheritdoc />
	public void Compile(CompilationContext context)
	{
		if (!(value is IAstRef) && !ReflectionHelper.IsNullable(value.ItemType))
		{
			context.Emit(OpCodes.Ldc_I4_1);
		}
		else if (ReflectionHelper.IsNullable(value.ItemType))
		{
			AstBuildHelper.ReadPropertyRV(new AstValueToAddr((IAstValue)value), value.ItemType.GetProperty("HasValue"))
			  .Compile(context);

			context.Emit(OpCodes.Ldc_I4_0);
			context.Emit(OpCodes.Ceq);
		}
		else
		{
			value.Compile(context);
			new AstConstantNull().Compile(context);
			context.Emit(OpCodes.Ceq);
		}
	}
}