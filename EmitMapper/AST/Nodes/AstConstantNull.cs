using EmitMapper.AST.Interfaces;
using System;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    internal class AstConstantNull : IAstRefOrValue
    {
        #region IAstReturnValueNode Members

        public Type ItemType => typeof(object);

        #endregion

        #region IAstNode Members

        public void Compile(CompilationContext context)
        {
            context.Emit(OpCodes.Ldnull);
        }

        #endregion
    }
}