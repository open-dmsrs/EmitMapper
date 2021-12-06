namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection.Emit;

using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

internal class AstReadArrayItem : IAstStackItem
{
    public IAstRef Array;

    public int Index;

    public Type ItemType => this.Array.ItemType.GetElementType();

    public virtual void Compile(CompilationContext context)
    {
        this.Array.Compile(context);
        context.Emit(OpCodes.Ldc_I4, this.Index);
        context.Emit(OpCodes.Ldelem, this.ItemType);
    }
}

internal class AstReadArrayItemRef : AstReadArrayItem, IAstRef
{
    public override void Compile(CompilationContext context)
    {
        CompilationHelper.CheckIsRef(this.ItemType);
        base.Compile(context);
    }
}

internal class AstReadArrayItemValue : AstReadArrayItem, IAstValue
{
    public override void Compile(CompilationContext context)
    {
        CompilationHelper.CheckIsValue(this.ItemType);
        base.Compile(context);
    }
}

internal class AstReadArrayItemAddr : AstReadArrayItem, IAstAddr
{
    public override void Compile(CompilationContext context)
    {
        CompilationHelper.CheckIsValue(this.ItemType);
        this.Array.Compile(context);
        context.Emit(OpCodes.Ldc_I4, this.Index);
        context.Emit(OpCodes.Ldelema, this.ItemType);
    }
}