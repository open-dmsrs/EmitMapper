namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection.Emit;

using EmitMapper.AST.Interfaces;

internal class AstExprNot : IAstValue
{
    private readonly IAstRefOrValue _value;

    public AstExprNot(IAstRefOrValue value)
    {
        this._value = value;
    }

    public Type ItemType => typeof(int);

    public void Compile(CompilationContext context)
    {
        context.Emit(OpCodes.Ldc_I4_0);
        this._value.Compile(context);
        context.Emit(OpCodes.Ceq);
    }
}