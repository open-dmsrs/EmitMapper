using System;
using System.Collections.Generic;
using System.Reflection;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

internal class AstCallMethod : IAstRefOrValue
{
  public List<IAstStackItem> Arguments;

  public IAstRefOrAddr InvocationObject;

  public MethodInfo MethodInfo;

  public AstCallMethod(MethodInfo methodInfo, IAstRefOrAddr invocationObject, List<IAstStackItem> arguments)
  {
    if (methodInfo == null)
      throw new InvalidOperationException("methodInfo is null");

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