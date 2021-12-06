using EmitMapper.AST.Interfaces;
using System;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    internal class AstTypeof : IAstRef
    {
        public Type Type;

        #region IAstStackItem Members

        public Type ItemType => typeof(Type);

        #endregion

        #region IAstNode Members

        public void Compile(CompilationContext context)
        {
            context.Emit(OpCodes.Ldtoken, Type);
            context.EmitCall(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
        }

        #endregion
    }
}
