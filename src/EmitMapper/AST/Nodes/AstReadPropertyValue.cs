namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast read property value.
/// </summary>
internal class AstReadPropertyValue : AstReadProperty, IAstValue
{
/// <inheritdoc />
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsValue(ItemType);
    base.Compile(context);
  }
}