using EmitMapper.AST.Interfaces;
using System;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    internal class AstValueToAddr : IAstAddr
    {
        public IAstValue Value;
        public Type ItemType => Value.ItemType;

        public AstValueToAddr(IAstValue value)
        {
            Value = value;
        }

        public void Compile(CompilationContext context)
        {
            LocalBuilder loc = context.ILGenerator.DeclareLocal(ItemType);
            new AstInitializeLocalVariable(loc).Compile(context);
            new AstWriteLocal()
            {
                LocalIndex = loc.LocalIndex,
                LocalType = loc.LocalType,
                Value = Value
            }.Compile(context);
            new AstReadLocalAddr(loc).Compile(context);
        }
    }
}