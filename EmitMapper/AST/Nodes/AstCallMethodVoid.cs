using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;
using EmitMapper.Utils;

namespace EmitMapper.AST.Nodes;

internal class AstCallMethodVoid : IAstNode
{
  protected IAstRefOrAddr InvocationObject;
  protected List<IAstStackItem> Arguments;

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

    if (MethodInfo.ReturnType != TypeHome.Void)
      context.Emit(OpCodes.Pop);
  }
}