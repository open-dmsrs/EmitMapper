namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast read property ref.
/// </summary>
internal class AstReadPropertyRef : AstReadProperty, IAstRef
{
/// <inheritdoc />
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsRef(ItemType);
    base.Compile(context);
  }
}