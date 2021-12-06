namespace EmitMapper.AST.Nodes;

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using EmitMapper.AST.Interfaces;

internal class AstCallMethodVoid : IAstNode
{
    protected List<IAstStackItem> Arguments;

    protected IAstRefOrAddr InvocationObject;

    protected MethodInfo MethodInfo;

    public AstCallMethodVoid(MethodInfo methodInfo, IAstRefOrAddr invocationObject, List<IAstStackItem> arguments)
    {
        this.MethodInfo = methodInfo;
        this.InvocationObject = invocationObject;
        this.Arguments = arguments;
    }

    public void Compile(CompilationContext context)
    {
        new AstCallMethod(this.MethodInfo, this.InvocationObject, this.Arguments).Compile(context);

        if (this.MethodInfo.ReturnType != typeof(void))
            context.Emit(OpCodes.Pop);
    }
}