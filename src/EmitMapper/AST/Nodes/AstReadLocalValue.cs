namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast read local value.
/// </summary>
internal class AstReadLocalValue : AstReadLocal, IAstValue
{
/// <inheritdoc />
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsValue(ItemType);
    base.Compile(context);
  }
}