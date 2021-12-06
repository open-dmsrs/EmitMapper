using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using System;

/* Unmerged change from project 'EmitMapper (netstandard2.1)'
Before:
using EmitMapper.AST.Helpers;
After:
using System.Reflection;
*/
using System.Reflection;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    internal class AstReadField : IAstStackItem
    {
        public IAstRefOrAddr SourceObject;
        public FieldInfo FieldInfo;

        public Type ItemType => FieldInfo.FieldType;

        public virtual void Compile(CompilationContext context)
        {
            SourceObject.Compile(context);
            context.Emit(OpCodes.Ldfld, FieldInfo);
        }
    }

    internal class AstReadFieldRef : AstReadField, IAstRef
    {
        public override void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsRef(ItemType);
            base.Compile(context);
        }
    }

    internal class AstReadFieldValue : AstReadField, IAstValue
    {
        public override void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(ItemType);
            base.Compile(context);
        }
    }

    internal class AstReadFieldAddr : AstReadField, IAstAddr
    {
        public override void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(ItemType);
            SourceObject.Compile(context);
            context.Emit(OpCodes.Ldflda, FieldInfo);
        }
    }
}