using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;


namespace EmitMapper.AST.Nodes
{
    internal class AstCallMethod : IAstRefOrValue
    {
        public MethodInfo MethodInfo;
        public IAstRefOrAddr InvocationObject;
        public List<IAstStackItem> Arguments;

        public AstCallMethod(
            MethodInfo methodInfo,
            IAstRefOrAddr invocationObject,
            List<IAstStackItem> arguments)
        {
            if (methodInfo == null)
            {
                throw new InvalidOperationException("methodInfo is null");
            }
            MethodInfo = methodInfo;
            InvocationObject = invocationObject;
            Arguments = arguments;
        }

        public Type ItemType => MethodInfo.ReturnType;

        public virtual void Compile(CompilationContext context)
        {
            CompilationHelper.EmitCall(context, InvocationObject, MethodInfo, Arguments);
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
            CompilationHelper.CheckIsRef(ItemType);
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
            CompilationHelper.CheckIsValue(ItemType);
            base.Compile(context);
        }
    }
}