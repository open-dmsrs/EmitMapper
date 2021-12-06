using EmitMapper.AST.Interfaces;
using System;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    internal class AstConstantInt32 : IAstValue
    {
        public int Value;

        #region IAstReturnValueNode Members

        public Type ItemType => typeof(int);

        #endregion

        #region IAstNode Members

        public void Compile(CompilationContext context)
        {
            context.Emit(OpCodes.Ldc_I4, Value);
        }

        #endregion
    }
}