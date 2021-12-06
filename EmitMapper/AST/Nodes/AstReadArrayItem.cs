
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
    internal class AstReadArrayItem : IAstStackItem
    {
        public IAstRef Array;
        public int Index;

        public Type ItemType => Array.ItemType.GetElementType();

        public virtual void Compile(CompilationContext context)
        {
            Array.Compile(context);
            context.Emit(OpCodes.Ldc_I4, Index);
            context.Emit(OpCodes.Ldelem, ItemType);
        }
    }

    internal class AstReadArrayItemRef : AstReadArrayItem, IAstRef
    {
        public override void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsRef(ItemType);
            base.Compile(context);
        }
    }

    internal class AstReadArrayItemValue : AstReadArrayItem, IAstValue
    {
        public override void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(ItemType);
            base.Compile(context);
        }
    }

    internal class AstReadArrayItemAddr : AstReadArrayItem, IAstAddr
    {
        public override void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(ItemType);
            Array.Compile(context);
            context.Emit(OpCodes.Ldc_I4, Index);
            context.Emit(OpCodes.Ldelema, ItemType);
        }
    }
}