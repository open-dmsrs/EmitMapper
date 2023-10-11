namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast read array item ref.
/// </summary>
internal class AstReadArrayItemRef : AstReadArrayItem, IAstRef
{
/// <inheritdoc />
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsRef(ItemType);
    base.Compile(context);
  }
}