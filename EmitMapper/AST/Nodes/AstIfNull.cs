namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection.Emit;

using EmitMapper.AST.Interfaces;

/// <summary>
///     Generates "value ?? ifNullValue" expression.
/// </summary>
internal class AstIfNull : IAstRefOrValue
{
    private readonly IAstRefOrValue _ifNullValue;

    private readonly IAstRef _value;

    public AstIfNull(IAstRef value, IAstRefOrValue ifNullValue)
    {
        this._value = value;
        this._ifNullValue = ifNullValue;
        if (!this._value.ItemType.IsAssignableFrom(this._ifNullValue.ItemType))
            throw new EmitMapperException("Incorrect ifnull expression");
    }

    public Type ItemType => this._value.ItemType;

    public void Compile(CompilationContext context)
    {
        var ifNotNullLabel = context.ILGenerator.DefineLabel();
        this._value.Compile(context);
        context.Emit(OpCodes.Dup);
        context.Emit(OpCodes.Brtrue_S, ifNotNullLabel);
        context.Emit(OpCodes.Pop);
        this._ifNullValue.Compile(context);
        context.ILGenerator.MarkLabel(ifNotNullLabel);
    }
}