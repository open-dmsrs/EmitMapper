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

/// <inheritdoc />
  public void Compile(CompilationContext context)
  {
    context.Emit(OpCodes.Ldtoken, Type);
    context.EmitCall(OpCodes.Call, Metadata<Type>.Type.GetMethodCache(nameof(Type.GetTypeFromHandle)));
  }
}