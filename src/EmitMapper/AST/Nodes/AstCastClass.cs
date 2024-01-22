namespace EmitMapper.AST.Nodes;

/// <summary>
/// The ast castclass.
/// </summary>
internal class AstCastclass : IAstRefOrValue
{
	protected Type TargetType;

	protected IAstRefOrValue Value;

	/// <summary>
	/// Initializes a new instance of the <see cref="AstCastclass"/> class.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="targetType">The target type.</param>
	public AstCastclass(IAstRefOrValue value, Type targetType)
	{
		this.Value = value;
		this.TargetType = targetType;
	}

	/// <summary>
	/// Gets the item type.
	/// </summary>
	public Type ItemType => TargetType;

	/// <inheritdoc/>
	/// <exception cref="EmitMapperException"></exception>
	public virtual void Compile(CompilationContext context)
	{
		if (Value.ItemType != TargetType)
		{
			switch (Value.ItemType.IsValueType)
			{
				case false when !TargetType.IsValueType:
					Value.Compile(context);
					context.Emit(OpCodes.Castclass, TargetType);

					return;
			}

			switch (TargetType.IsValueType)
			{
				case true when !Value.ItemType.IsValueType:
					new AstUnbox { RefObj = (IAstRef)Value, UnboxedType = TargetType }.Compile(context);

					return;
				default:
					throw new EmitMapperException();
			}
		}

		Value.Compile(context);
	}
}