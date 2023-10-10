namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast read argument value.
/// </summary>
internal class AstReadArgumentValue : AstReadArgument, IAstValue
{
  /// <summary>
  /// </summary>
  /// <param name="context">The context.</param>
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsValue(ItemType);
    base.Compile(context);
  }
}