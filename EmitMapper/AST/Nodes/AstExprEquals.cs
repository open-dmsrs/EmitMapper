using EmitMapper.AST.Interfaces;
using System;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
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
            leftValue.Compile(context);
            rightValue.Compile(context);
            context.Emit(OpCodes.Ceq);
        }

        #endregion
    }
}