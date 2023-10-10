namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast read field addr.
/// </summary>
internal class AstReadFieldAddr : AstReadField, IAstAddr
{
  /// <summary>
  /// </summary>
  /// <param name="context">The context.</param>
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsValue(ItemType);
    SourceObject.Compile(context);
    context.Emit(OpCodes.Ldflda, FieldInfo);
  }
}