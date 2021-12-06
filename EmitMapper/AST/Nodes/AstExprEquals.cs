namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection.Emit;

using EmitMapper.AST.Interfaces;

internal class AstExprEquals : IAstValue
{
    private readonly IAstValue leftValue;

    private readonly IAstValue rightValue;

    public AstExprEquals(IAstValue leftValue, IAstValue rightValue)
    {
        this.leftValue = leftValue;
        this.rightValue = rightValue;
    }

    #region IAstReturnValueNode Members

    public Type ItemType => typeof(int);

    #endregion

    #region IAstNode Members

    public void Compile(CompilationContext context)
    {
        this.leftValue.Compile(context);
        this.rightValue.Compile(context);
        context.Emit(OpCodes.Ceq);
    }

    #endregion
}