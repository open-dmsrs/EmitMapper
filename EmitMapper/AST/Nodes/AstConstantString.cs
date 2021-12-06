using EmitMapper.AST.Interfaces;
using System;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    internal class AstConstantString : IAstRef
    {
        public string Str;

        #region IAstStackItem Members

        public Type ItemType => typeof(string);

        #endregion

        #region IAstNode Members

        public void Compile(CompilationContext context)
        {
            context.Emit(OpCodes.Ldstr, Str);
        }

        #endregion
    }
}