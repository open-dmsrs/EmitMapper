namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast read this ref.
/// </summary>
internal class AstReadThisRef : AstReadThis, IAstRef
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