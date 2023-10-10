namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast constant null.
/// </summary>
internal class AstConstantNull : IAstRefOrValue
{
  /// <summary>
  ///   Gets the item type.
  /// </summary>
  public Type ItemType => Metadata<object>.Type;

  /// <summary>
  /// </summary>
  /// <param name="context">The context.</param>
  public void Compile(CompilationContext context)
  {
    context.Emit(OpCodes.Ldnull);
  }
}