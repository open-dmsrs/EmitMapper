namespace EmitMapper.AST.Nodes;

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using EmitMapper.AST.Interfaces;
using EmitMapper.Utils;

internal class AstCallMethodVoid : IAstNode
{
  protected List<IAstStackItem> Arguments;

  protected IAstRefOrAddr InvocationObject;

  protected MethodInfo MethodInfo;

  public AstCallMethodVoid(MethodInfo methodInfo, IAstRefOrAddr invocationObject, List<IAstStackItem> arguments)
  {
    MethodInfo = methodInfo;
    InvocationObject = invocationObject;
    Arguments = arguments;
  }

  public void Compile(CompilationContext context)
  {
    new AstCallMethod(MethodInfo, InvocationObject, Arguments).Compile(context);

    if (MethodInfo.ReturnType != Metadata.Void)
      context.Emit(OpCodes.Pop);
  }
}