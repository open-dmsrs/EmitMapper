namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection.Emit;

using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using EmitMapper.Utils;

internal class AstExprIsNull : IAstValue
{
    private readonly IAstRefOrValue value;

    public AstExprIsNull(IAstRefOrValue value)
    {
        this.value = value;
    }

    #region IAstReturnValueNode Members

    public Type ItemType => typeof(int);

    #endregion

    #region IAstNode Members

    public void Compile(CompilationContext context)
    {
        if (!(this.value is IAstRef) && !ReflectionUtils.IsNullable(this.value.ItemType))
        {
            context.Emit(OpCodes.Ldc_I4_1);
        }
        else if (ReflectionUtils.IsNullable(this.value.ItemType))
        {
            AstBuildHelper.ReadPropertyRV(
                new AstValueToAddr((IAstValue)this.value),
                this.value.ItemType.GetProperty("HasValue")).Compile(context);
            context.Emit(OpCodes.Ldc_I4_0);
            context.Emit(OpCodes.Ceq);
        }
        else
        {
            this.value.Compile(context);
            new AstConstantNull().Compile(context);
            context.Emit(OpCodes.Ceq);
        }
    }

    #endregion
}