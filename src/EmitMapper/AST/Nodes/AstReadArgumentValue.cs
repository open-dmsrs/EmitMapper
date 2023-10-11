namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast read argument value.
/// </summary>
internal class AstReadArgumentValue : AstReadArgument, IAstValue
{
/// <inheritdoc />
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsValue(ItemType);
    base.Compile(context);
  }
}