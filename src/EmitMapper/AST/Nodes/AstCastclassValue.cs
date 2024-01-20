namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast castclass value.
/// </summary>
internal class AstCastclassValue : AstCastclass, IAstValue
{
	/// <summary>
	///   Initializes a new instance of the <see cref="AstCastclassValue" /> class.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="targetType">The target type.</param>
	public AstCastclassValue(IAstRefOrValue value, Type targetType)
	  : base(value, targetType)
	{
	}

	/// <inheritdoc />
	public override void Compile(CompilationContext context)
	{
		CompilationHelper.CheckIsValue(ItemType);
		base.Compile(context);
	}
}