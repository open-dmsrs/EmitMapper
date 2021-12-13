namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection.Emit;

using EmitMapper.AST.Interfaces;

internal class AstExprEquals : IAstValue
{
    private readonly IAstValue _leftValue;

    private readonly IAstValue _rightValue;

    public AstExprEquals(IAstValue leftValue, IAstValue rightValue)
    {
        this._leftValue = leftValue;
        this._rightValue = rightValue;
    }

    #region IAstReturnValueNode Members

    public Type ItemType => typeof(int);

    #endregion

    #region IAstNode Members

    public void Compile(CompilationContext context)
    {
        this._leftValue.Compile(context);
        this._rightValue.Compile(context);
        context.Emit(OpCodes.Ceq);
    }

    #endregion
}