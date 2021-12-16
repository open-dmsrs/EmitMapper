using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

/// <summary>
///     Generates "value ?? ifNullValue" expression.
/// </summary>
internal class AstIfNull : IAstRefOrValue
{
    private readonly IAstRef _value;
    private readonly IAstRefOrValue _ifNullValue;

    public AstIfNull(IAstRef value, IAstRefOrValue ifNullValue)
    {
        _value = value;
        _ifNullValue = ifNullValue;
        if (!_value.ItemType.IsAssignableFrom(_ifNullValue.ItemType))
            throw new EmitMapperException("Incorrect ifnull expression");
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