namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast read argument ref.
/// </summary>
internal class AstReadArgumentRef : AstReadArgument, IAstRef
{
/// <inheritdoc />
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsRef(ItemType);
    base.Compile(context);
  }
}