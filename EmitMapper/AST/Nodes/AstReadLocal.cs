namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection.Emit;

using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

internal class AstReadLocal : IAstStackItem
{
    public int LocalIndex;

    public Type LocalType;

    public AstReadLocal()
    {
    }

    public AstReadLocal(LocalBuilder loc)
    {
        this.LocalIndex = loc.LocalIndex;
        this.LocalType = loc.LocalType;
    }

    public Type ItemType => this.LocalType;

    public virtual void Compile(CompilationContext context)
    {
        context.Emit(OpCodes.Ldloc, this.LocalIndex);
    }
}

internal class AstReadLocalRef : AstReadLocal, IAstRef
{
    public override void Compile(CompilationContext context)
    {
        CompilationHelper.CheckIsRef(this.ItemType);
        base.Compile(context);
    }
}

internal class AstReadLocalValue : AstReadLocal, IAstValue
{
    public override void Compile(CompilationContext context)
    {
        CompilationHelper.CheckIsValue(this.ItemType);
        base.Compile(context);
    }
}

internal class AstReadLocalAddr : AstReadLocal, IAstAddr
{
    public AstReadLocalAddr(LocalBuilder loc)
    {
        this.LocalIndex = loc.LocalIndex;
        this.LocalType = loc.LocalType.MakeByRefType();
    }

    public override void Compile(CompilationContext context)
    {
        //CompilationHelper.CheckIsValue(itemType);
        context.Emit(OpCodes.Ldloca, this.LocalIndex);
    }
}