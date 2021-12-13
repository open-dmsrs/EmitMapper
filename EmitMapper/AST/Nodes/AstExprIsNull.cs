namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection.Emit;

using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using EmitMapper.Utils;

internal class AstExprIsNull : IAstValue
{
    private readonly IAstRefOrValue _value;

    public AstExprIsNull(IAstRefOrValue value)
    {
        this._value = value;
    }

    #region IAstReturnValueNode Members

    public Type ItemType => typeof(int);

    #endregion

    #region IAstNode Members

    public void Compile(CompilationContext context)
    {
        if (!(this._value is IAstRef) && !ReflectionUtils.IsNullable(this._value.ItemType))
        {
            context.Emit(OpCodes.Ldc_I4_1);
        }
        else if (ReflectionUtils.IsNullable(this._value.ItemType))
        {
            AstBuildHelper.ReadPropertyRV(
                new AstValueToAddr((IAstValue)this._value),
                this._value.ItemType.GetProperty("HasValue")).Compile(context);
            context.Emit(OpCodes.Ldc_I4_0);
            context.Emit(OpCodes.Ceq);
        }
        else
        {
            this._value.Compile(context);
            new AstConstantNull().Compile(context);
            context.Emit(OpCodes.Ceq);
        }
    }

    #endregion
}