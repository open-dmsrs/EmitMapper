namespace EmitMapper.AST.Nodes;

/// <summary>
/// The ast castclass ref.
/// </summary>
internal class AstCastclassRef : AstCastclass, IAstRef
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AstCastclassRef"/> class.
  /// </summary>
  /// <param name="value">The value.</param>
  /// <param name="targetType">The target type.</param>
  public AstCastclassRef(IAstRefOrValue value, Type targetType)
    : base(value, targetType)
  {
  }

  /// <inheritdoc/>
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsRef(ItemType);
    base.Compile(context);
  }
}