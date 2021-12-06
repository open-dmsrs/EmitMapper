
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
    internal abstract class AstIndirectRead : IAstStackItem
    {
        public Type ArgumentType;

        public Type ItemType => ArgumentType;

        public abstract void Compile(CompilationContext context);
    }

    internal class AstIndirectReadRef : AstIndirectRead, IAstRef
    {
        public override void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsRef(ItemType);
            context.Emit(OpCodes.Ldind_Ref, ItemType);
        }
    }

    internal class AstIndirectReadValue : AstIndirectRead, IAstValue
    {
        public override void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(ItemType);
            if (ItemType == typeof(int))
            {
                context.Emit(OpCodes.Ldind_I4);
            }
            else
            {
                throw new Exception("Unsupported type");
            }
        }
    }

    internal class AstIndirectReadAddr : AstIndirectRead, IAstAddr
    {
        public override void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(ItemType);
        }
    }
}