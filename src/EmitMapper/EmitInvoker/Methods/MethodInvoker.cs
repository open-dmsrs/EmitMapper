using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EmitMapper.AST;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using EmitMapper.AST.Nodes;
using EmitMapper.Utils;

namespace EmitMapper.EmitInvoker.Methods;

public static class MethodInvoker
{
  private static readonly LazyConcurrentDictionary<string, Type> Cache = new();

  public static MethodInvokerBase GetMethodInvoker(object targetObject, MethodInfo mi)
  {
    var typeName = "EmitMapper.MethodCaller_" + mi;

    var type = Cache.GetOrAdd(
      typeName,
      _ =>
      {
        if (mi.ReturnType == Metadata.Void)
          return BuildActionCallerType(typeName, mi);

        return BuildFuncCallerType(typeName, mi);
      });

    var result = (MethodInvokerBase)ObjectFactory.CreateInstance(type);
    result.TargetObject = targetObject;

    return result;
  }

  private static Type BuildActionCallerType(string typeName, MethodInfo mi)
  {
    var par = mi.GetParameters();

    var actionCallerType = par.Length switch
    {
      0 => Metadata<MethodInvokerAction0>.Type,
      1 => Metadata<MethodInvokerAction1>.Type,
      2 => Metadata<MethodInvokerAction2>.Type,
      3 => Metadata<MethodInvokerAction3>.Type,
      _ => throw new EmitMapperException("too many method parameters")
    };

    var tb = DynamicAssemblyManager.DefineType(typeName, actionCallerType);

    var methodBuilder = tb.DefineMethod(
      "CallAction",
      MethodAttributes.Public | MethodAttributes.Virtual,
      null,
      Enumerable.Repeat(Metadata<object>.Type, par.Length).ToArray());

    new AstComplexNode { Nodes = new List<IAstNode> { CreateCallMethod(mi, par), new AstReturnVoid() } }.Compile(
      new CompilationContext(methodBuilder.GetILGenerator()));

    return tb.CreateType();
  }

  private static Type BuildFuncCallerType(string typeName, MethodInfo mi)
  {
    var par = mi.GetParameters();

    var funcCallerType = par.Length switch
    {
      0 => Metadata<MethodInvokerFunc0>.Type,
      1 => Metadata<MethodInvokerFunc1>.Type,
      2 => Metadata<MethodInvokerFunc2>.Type,
      3 => Metadata<MethodInvokerFunc3>.Type,
      _ => throw new EmitMapperException("too many method parameters")
    };

    var tb = DynamicAssemblyManager.DefineType(typeName, funcCallerType);

    var methodBuilder = tb.DefineMethod(
      "CallFunc",
      MethodAttributes.Public | MethodAttributes.Virtual,
      Metadata<object>.Type,
      Enumerable.Repeat(Metadata<object>.Type, par.Length).ToArray());

    new AstReturn { ReturnType = Metadata<object>.Type, ReturnValue = CreateCallMethod(mi, par) }.Compile(
      new CompilationContext(methodBuilder.GetILGenerator()));

    return tb.CreateType();
  }

  private static IAstRefOrValue CreateCallMethod(MethodInfo mi, IEnumerable<ParameterInfo> parameters)
  {
    return AstBuildHelper.CallMethod(
      mi,
      mi.IsStatic
        ? null
        : new AstCastclassRef(
          AstBuildHelper.ReadFieldRV(
            new AstReadThis { ThisType = Metadata<MethodInvokerBase>.Type },
            Metadata<MethodInvokerBase>.Type.GetField(
              nameof(MethodInvokerBase.TargetObject),
              BindingFlags.Public | BindingFlags.Instance)),
          mi.DeclaringType),
      parameters.Select((p, idx) => (IAstStackItem)AstBuildHelper.ReadArgumentRV(idx + 1, Metadata<object>.Type))
        .ToList());
  }
}