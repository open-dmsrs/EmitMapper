namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast indirect read ref.
/// </summary>
internal class AstIndirectReadRef : AstIndirectRead, IAstRef
{
/// <inheritdoc />
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsRef(ItemType);
    context.Emit(OpCodes.Ldind_Ref, ItemType);
  }
}