namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast unbox.
/// </summary>
internal class AstUnbox : IAstValue
{
  public IAstRef RefObj;

  public Type UnboxedType;

  /// <summary>
  ///   Gets the item type.
  /// </summary>
  public Type ItemType => UnboxedType;

/// <inheritdoc />
  public void Compile(CompilationContext context)
  {
    RefObj.Compile(context);
    context.Emit(OpCodes.Unbox_Any, UnboxedType);
  }
}