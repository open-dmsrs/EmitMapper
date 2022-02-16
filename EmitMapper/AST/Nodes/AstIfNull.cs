namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection.Emit;

using EmitMapper.AST.Interfaces;

/// <summary>
///   Generates "value ?? ifNullValue" expression.
/// </summary>
internal class AstIfNull : IAstRefOrValue
{
  private readonly IAstRefOrValue _ifNullValue;

  private readonly IAstRef _value;

  public AstIfNull(IAstRef value, IAstRefOrValue ifNullValue)
  {
    _value = value;
    _ifNullValue = ifNullValue;
    if (!_value.ItemType.IsAssignableFrom(_ifNullValue.ItemType))
      throw new EmitMapperException("Incorrect if null expression");
  }

  public Type ItemType => _value.ItemType;

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