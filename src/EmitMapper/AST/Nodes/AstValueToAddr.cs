using System;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;
/// <summary>
/// The ast value to addr.
/// </summary>

internal class AstValueToAddr : IAstAddr
{
  public IAstValue Value;

  /// <summary>
  /// Initializes a new instance of the <see cref="AstValueToAddr"/> class.
  /// </summary>
  /// <param name="value">The value.</param>
  public AstValueToAddr(IAstValue value)
  {
    Value = value;
  }

  /// <summary>
  /// Gets the item type.
  /// </summary>
  public Type ItemType => Value.ItemType;

  /// <summary>
  /// 
  /// </summary>
  /// <param name="context">The context.</param>
  public void Compile(CompilationContext context)
  {
    var loc = context.ILGenerator.DeclareLocal(ItemType);
    new AstInitializeLocalVariable(loc).Compile(context);
    new AstWriteLocal { LocalIndex = loc.LocalIndex, LocalType = loc.LocalType, Value = Value }.Compile(context);
    new AstReadLocalAddr(loc).Compile(context);
  }
}