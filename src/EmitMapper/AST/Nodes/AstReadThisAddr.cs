namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast read this addr.
/// </summary>
internal class AstReadThisAddr : AstReadThis, IAstRef
{
/// <inheritdoc />
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsRef(ItemType);
    base.Compile(context);
  }
}