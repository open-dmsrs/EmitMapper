
/* Unmerged change from project 'EmitMapper (netstandard2.1)'
Before:
using System;
using System.Reflection.Emit;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
After:
using EmitMapper.AST.Helpers;
using EmitMapper.Reflection.Interfaces;
using System;
using System.AST.Interfaces;
*/
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using System;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    internal class AstReadLocal : IAstStackItem
    {
        public int LocalIndex;
        public Type LocalType;

        public Type ItemType => LocalType;

        public AstReadLocal()
        {
        }

        public AstReadLocal(LocalBuilder loc)
        {
            LocalIndex = loc.LocalIndex;
            LocalType = loc.LocalType;
        }

        public virtual void Compile(CompilationContext context)
        {
            context.Emit(OpCodes.Ldloc, LocalIndex);
        }
    }

    internal class AstReadLocalRef : AstReadLocal, IAstRef
    {
        public override void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsRef(ItemType);
            base.Compile(context);
        }
    }

    internal class AstReadLocalValue : AstReadLocal, IAstValue
    {
        public override void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(ItemType);
            base.Compile(context);
        }
    }

    internal class AstReadLocalAddr : AstReadLocal, IAstAddr
    {
        public AstReadLocalAddr(LocalBuilder loc)
        {
            LocalIndex = loc.LocalIndex;
            LocalType = loc.LocalType.MakeByRefType();
        }

        public override void Compile(CompilationContext context)
        {
            //CompilationHelper.CheckIsValue(itemType);
            context.Emit(OpCodes.Ldloca, LocalIndex);
        }
    }
}