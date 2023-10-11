namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast value to addr.
/// </summary>
internal class AstValueToAddr : IAstAddr
{
  public IAstValue Value;

  /// <summary>
  ///   Initializes a new instance of the <see cref="AstValueToAddr" /> class.
  /// </summary>
  /// <param name="value">The value.</param>
  public AstValueToAddr(IAstValue value)
  {
    Value = value;
  }

  /// <summary>
  ///   Gets the item type.
  /// </summary>
  public Type ItemType => Value.ItemType;

/// <inheritdoc />
  public void Compile(CompilationContext context)
  {
    var loc = context.ILGenerator.DeclareLocal(ItemType);
    new AstInitializeLocalVariable(loc).Compile(context);
    new AstWriteLocal { LocalIndex = loc.LocalIndex, LocalType = loc.LocalType, Value = Value }.Compile(context);
    new AstReadLocalAddr(loc).Compile(context);
  }
}