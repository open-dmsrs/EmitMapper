namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast read argument addr.
/// </summary>
internal class AstReadArgumentAddr : AstReadArgument, IAstAddr
{
/// <inheritdoc />
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsValue(ItemType);
    context.Emit(OpCodes.Ldarga, ArgumentIndex);
  }
}