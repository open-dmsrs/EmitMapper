namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast read local ref.
/// </summary>
internal class AstReadLocalRef : AstReadLocal, IAstRef
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