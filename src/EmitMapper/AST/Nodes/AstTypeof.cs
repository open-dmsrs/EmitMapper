namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast typeof.
/// </summary>
internal class AstTypeof : IAstRef
{
  public Type Type;

  /// <summary>
  ///   Gets the item type.
  /// </summary>
  public Type ItemType => Metadata<Type>.Type;

  /// <summary>
  /// </summary>
  /// <param name="context">The context.</param>
  public void Compile(CompilationContext context)
  {
    context.Emit(OpCodes.Ldtoken, Type);
    context.EmitCall(OpCodes.Call, Metadata<Type>.Type.GetMethodCache(nameof(Type.GetTypeFromHandle)));
  }
}