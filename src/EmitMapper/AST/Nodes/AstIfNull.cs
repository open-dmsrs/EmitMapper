using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

/// <summary>
///   Generates "value ?? ifNullValue" expression.
/// </summary>
internal class AstIfNull : IAstRefOrValue
{
  private readonly IAstRefOrValue _ifNullValue;

  private readonly IAstRef _value;

  /// <summary>
  /// Initializes a new instance of the <see cref="AstIfNull"/> class.
  /// </summary>
  /// <param name="value">The value.</param>
  /// <param name="ifNullValue">The if null value.</param>
  public AstIfNull(IAstRef value, IAstRefOrValue ifNullValue)
  {
    _value = value;
    _ifNullValue = ifNullValue;

    if (!_value.ItemType.IsAssignableFrom(_ifNullValue.ItemType))
      throw new EmitMapperException("Incorrect if null expression");
  }

  /// <summary>
  /// Gets the item type.
  /// </summary>
  public Type ItemType => _value.ItemType;

  /// <summary>
  /// 
  /// </summary>
  /// <param name="context">The context.</param>
  public void Compile(CompilationContext context)
  {
    var ifNotNullLabel = context.ILGenerator.DefineLabel();
    _value.Compile(context);
    context.Emit(OpCodes.Dup);
    context.Emit(OpCodes.Brtrue_S, ifNotNullLabel);
    context.Emit(OpCodes.Pop);
    _ifNullValue.Compile(context);
    context.ILGenerator.MarkLabel(ifNotNullLabel);
  }
}