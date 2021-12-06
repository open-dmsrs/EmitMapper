namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection;

using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

internal class AstReadProperty : IAstRefOrValue
{
    public PropertyInfo PropertyInfo;

    public IAstRefOrAddr SourceObject;

    public Type ItemType => this.PropertyInfo.PropertyType;

    public virtual void Compile(CompilationContext context)
    {
        var mi = this.PropertyInfo.GetGetMethod();

        if (mi == null)
            throw new Exception("Property " + this.PropertyInfo.Name + " doesn't have get accessor");

        AstBuildHelper.CallMethod(mi, this.SourceObject, null).Compile(context);
    }
}

internal class AstReadPropertyRef : AstReadProperty, IAstRef
{
    public override void Compile(CompilationContext context)
    {
        CompilationHelper.CheckIsRef(this.ItemType);
        base.Compile(context);
    }
}

internal class AstReadPropertyValue : AstReadProperty, IAstValue
{
    public override void Compile(CompilationContext context)
    {
        CompilationHelper.CheckIsValue(this.ItemType);
        base.Compile(context);
    }
}