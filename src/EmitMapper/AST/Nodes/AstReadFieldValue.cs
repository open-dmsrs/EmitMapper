namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast read field value.
/// </summary>
internal class AstReadFieldValue : AstReadField, IAstValue
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