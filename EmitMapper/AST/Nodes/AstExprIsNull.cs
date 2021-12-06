using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using EmitMapper.Utils;
using System;

/* Unmerged change from project 'EmitMapper (netstandard2.1)'
Before:
using EmitMapper.Utils;
After:
using System.Reflection.Emit;
*/
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
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
            if (!(value is IAstRef) && !ReflectionUtils.IsNullable(value.ItemType))
            {
                context.Emit(OpCodes.Ldc_I4_1);
            }
            else if (ReflectionUtils.IsNullable(value.ItemType))
            {
                AstBuildHelper.ReadPropertyRV(
                    new AstValueToAddr((IAstValue)value),
                    value.ItemType.GetProperty("HasValue")
                ).Compile(context);
                context.Emit(OpCodes.Ldc_I4_0);
                context.Emit(OpCodes.Ceq);
            }
            else
            {
                value.Compile(context);
                new AstConstantNull().Compile(context);
                context.Emit(OpCodes.Ceq);
            }
        }

        #endregion
    }
}
