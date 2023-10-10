namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast read property ref.
/// </summary>
internal class AstReadPropertyRef : AstReadProperty, IAstRef
{
  /// <summary>
  /// </summary>
  /// <param name="context">The context.</param>
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsRef(ItemType);
    base.Compile(context);
  }
}