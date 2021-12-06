namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection;
using System.Reflection.Emit;

using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

internal class AstReadField : IAstStackItem
{
    public FieldInfo FieldInfo;

    public IAstRefOrAddr SourceObject;

    public Type ItemType => this.FieldInfo.FieldType;

    public virtual void Compile(CompilationContext context)
    {
        this.SourceObject.Compile(context);
        context.Emit(OpCodes.Ldfld, this.FieldInfo);
    }
}

internal class AstReadFieldRef : AstReadField, IAstRef
{
    public override void Compile(CompilationContext context)
    {
        CompilationHelper.CheckIsRef(this.ItemType);
        base.Compile(context);
    }
}

internal class AstReadFieldValue : AstReadField, IAstValue
{
    public override void Compile(CompilationContext context)
    {
        CompilationHelper.CheckIsValue(this.ItemType);
        base.Compile(context);
    }
}

internal class AstReadFieldAddr : AstReadField, IAstAddr
{
    public override void Compile(CompilationContext context)
    {
        CompilationHelper.CheckIsValue(this.ItemType);
        this.SourceObject.Compile(context);
        context.Emit(OpCodes.Ldflda, this.FieldInfo);
    }
}