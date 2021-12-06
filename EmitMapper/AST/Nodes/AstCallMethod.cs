namespace EmitMapper.AST.Nodes;

using System;
using System.Collections.Generic;
using System.Reflection;

using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

internal class AstCallMethod : IAstRefOrValue
{
    public List<IAstStackItem> Arguments;

    public IAstRefOrAddr InvocationObject;

    public MethodInfo MethodInfo;

    public AstCallMethod(MethodInfo methodInfo, IAstRefOrAddr invocationObject, List<IAstStackItem> arguments)
    {
        if (methodInfo == null)
            throw new InvalidOperationException("methodInfo is null");
        this.MethodInfo = methodInfo;
        this.InvocationObject = invocationObject;
        this.Arguments = arguments;
    }

    public Type ItemType => this.MethodInfo.ReturnType;

    public virtual void Compile(CompilationContext context)
    {
        CompilationHelper.EmitCall(context, this.InvocationObject, this.MethodInfo, this.Arguments);
    }
}

internal class AstCallMethodRef : AstCallMethod, IAstRef
{
    public AstCallMethodRef(MethodInfo methodInfo, IAstRefOrAddr invocationObject, List<IAstStackItem> arguments)
        : base(methodInfo, invocationObject, arguments)
    {
    }

    public override void Compile(CompilationContext context)
    {
        CompilationHelper.CheckIsRef(this.ItemType);
        base.Compile(context);
    }
}

internal class AstCallMethodValue : AstCallMethod, IAstValue
{
    public AstCallMethodValue(MethodInfo methodInfo, IAstRefOrAddr invocationObject, List<IAstStackItem> arguments)
        : base(methodInfo, invocationObject, arguments)
    {
    }

    public override void Compile(CompilationContext context)
    {
        CompilationHelper.CheckIsValue(this.ItemType);
        base.Compile(context);
    }
}