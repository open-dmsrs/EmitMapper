using EmitMapper.AST.Interfaces;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    internal class AstCallMethodVoid : IAstNode
    {
        protected MethodInfo MethodInfo;
        protected IAstRefOrAddr InvocationObject;
        protected List<IAstStackItem> Arguments;

        public AstCallMethodVoid(
            MethodInfo methodInfo,
            IAstRefOrAddr invocationObject,
            List<IAstStackItem> arguments)
        {
            MethodInfo = methodInfo;
            InvocationObject = invocationObject;
            Arguments = arguments;
        }

        public void Compile(CompilationContext context)
        {
            new AstCallMethod(MethodInfo, InvocationObject, Arguments).Compile(context);

            if (MethodInfo.ReturnType != typeof(void))
            {
                context.Emit(OpCodes.Pop);
            }
        }
    }
}