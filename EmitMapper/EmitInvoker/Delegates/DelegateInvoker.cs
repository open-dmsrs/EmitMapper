using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EmitMapper.AST;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using EmitMapper.AST.Nodes;
using EmitMapper.Utils;

namespace EmitMapper.EmitInvoker.Delegates;

public static class DelegateInvoker
{
  public static DelegateInvokerBase GetDelegateInvoker(Delegate del)
  {
    var typeName = "EmitMapper.DelegateCaller_" + del;

    var creator = ThreadSaveCache.GetCreator(
      typeName,
      key =>
      {
        if (del.Method.ReturnType == Metadata.Void)
          return BuildActionCallerType(key, del);
        return BuildFuncCallerType(key, del);
      });
    var result = (DelegateInvokerBase)creator();
    result.Del = del;
    return result;
  }

  private static Type BuildFuncCallerType(string typeName, Delegate del)
  {
    var par = del.Method.GetParameters();
    var funcCallerType = par.Length switch
    {
      0 => Metadata<DelegateInvokerFunc0>.Type,
      1 => Metadata<DelegateInvokerFunc1>.Type,
      2 => Metadata<DelegateInvokerFunc2>.Type,
      3 => Metadata<DelegateInvokerFunc3>.Type,
      _ => throw new EmitMapperException("too many method parameters")
    };

    var tb = DynamicAssemblyManager.DefineType(typeName, funcCallerType);

    var methodBuilder = tb.DefineMethod(
      "CallFunc",
      MethodAttributes.Public | MethodAttributes.Virtual,
      Metadata<object>.Type,
      Enumerable.Repeat(Metadata<object>.Type, par.Length).ToArray());

    new AstReturn { ReturnType = Metadata<object>.Type, ReturnValue = CreateCallDelegate(del, par) }.Compile(
      new CompilationContext(methodBuilder.GetILGenerator()));

    return tb.CreateType();
  }

  private static Type BuildActionCallerType(string typeName, Delegate del)
  {
    var par = del.Method.GetParameters();
    var actionCallerType = par.Length switch
    {
      0 => Metadata<DelegateInvokerAction0>.Type,
      1 => Metadata<DelegateInvokerAction1>.Type,
      2 => Metadata<DelegateInvokerAction2>.Type,
      3 => Metadata<DelegateInvokerAction3>.Type,
      _ => throw new EmitMapperException("too many method parameters")
    };

    var tb = DynamicAssemblyManager.DefineType(typeName, actionCallerType);

    var methodBuilder = tb.DefineMethod(
      "CallAction",
      MethodAttributes.Public | MethodAttributes.Virtual,
      null,
      Enumerable.Repeat(Metadata<object>.Type, par.Length).ToArray());

    new AstComplexNode { Nodes = new List<IAstNode> { CreateCallDelegate(del, par), new AstReturnVoid() } }.Compile(
      new CompilationContext(methodBuilder.GetILGenerator()));

    return tb.CreateType();
  }

  private static IAstRefOrValue CreateCallDelegate(Delegate del, ParameterInfo[] parameters)
  {
    return AstBuildHelper.CallMethod(
      del.GetType().GetMethod("Invoke"),
      new AstCastclassRef(
        AstBuildHelper.ReadFieldRV(
          new AstReadThis { ThisType = Metadata<DelegateInvokerBase>.Type },
          Metadata<DelegateInvokerBase>.Type.GetField(
            nameof(DelegateInvokerBase.Del),
            BindingFlags.Public | BindingFlags.Instance)),
        del.GetType()),
      parameters.Select((p, idx) => (IAstStackItem)AstBuildHelper.ReadArgumentRV(idx + 1, Metadata<object>.Type))
        .ToList());
  }
}