using EmitMapper.AST.Interfaces;
using System;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    internal class AstUnbox : IAstValue
    {
        public Type UnboxedType;
        public IAstRef RefObj;

        public Type ItemType => UnboxedType;

        public void Compile(CompilationContext context)
        {
            RefObj.Compile(context);
            context.Emit(OpCodes.Unbox_Any, UnboxedType);
        }
    }
}