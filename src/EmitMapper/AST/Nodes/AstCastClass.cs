namespace EmitMapper.AST.Nodes;

/// <summary>
/// The ast castclass.
/// </summary>
internal class AstCastclass : IAstRefOrValue
{
	protected Type targetType;

	protected IAstRefOrValue value;

	/// <summary>
	/// Initializes a new instance of the <see cref="AstCastclass"/> class.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="targetType">The target type.</param>
	public AstCastclass(IAstRefOrValue value, Type targetType)
	{
		this.value = value;
		this.targetType = targetType;
	}

	/// <summary>
	/// Gets the item type.
	/// </summary>
	public Type ItemType => targetType;

	/// <inheritdoc/>
	/// <exception cref="EmitMapperException"></exception>
	public virtual void Compile(CompilationContext context)
	{
		if (value.ItemType != targetType)
		{
			if (!value.ItemType.IsValueType && !targetType.IsValueType)
			{
				value.Compile(context);
				context.Emit(OpCodes.Castclass, targetType);

				return;
			}

			if (targetType.IsValueType && !value.ItemType.IsValueType)
			{
				new AstUnbox { RefObj = (IAstRef)value, UnboxedType = targetType }.Compile(context);

				return;
			}

			throw new EmitMapperException();
		}

		value.Compile(context);
	}
}