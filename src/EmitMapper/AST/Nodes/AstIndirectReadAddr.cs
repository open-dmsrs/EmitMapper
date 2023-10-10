namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast indirect read addr.
/// </summary>
internal class AstIndirectReadAddr : AstIndirectRead, IAstAddr
{
  /// <summary>
  /// </summary>
  /// <param name="context">The context.</param>
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsValue(ItemType);
  }
}