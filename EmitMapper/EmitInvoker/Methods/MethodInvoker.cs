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
    public static MethodInvokerBase GetMethodInvoker(object targetObject, MethodInfo mi)
    {
        var typeName = "EmitMapper.MethodCaller_" + mi;

        var creator = ThreadSaveCache.GetCreator(
            typeName,
            _ =>
            {
                if (mi.ReturnType == typeof(void))
                    return BuildActionCallerType(typeName, mi);
                return BuildFuncCallerType(typeName, mi);
            });

        var result = (MethodInvokerBase)creator();
        result.TargetObject = targetObject;
        return result;
    }

    private static Type BuildFuncCallerType(string typeName, MethodInfo mi)
    {
        var par = mi.GetParameters();
        Type funcCallerType = par.Length switch
        {
            0 => typeof(MethodInvokerFunc0),
            1 => typeof(MethodInvokerFunc1),
            2 => typeof(MethodInvokerFunc2),
            3 => typeof(MethodInvokerFunc3),
            _ => throw new EmitMapperException("too many method parameters")
        };

        var tb = DynamicAssemblyManager.DefineType(typeName, funcCallerType);

        var methodBuilder = tb.DefineMethod(
            "CallFunc",
            MethodAttributes.Public | MethodAttributes.Virtual,
            typeof(object),
            Enumerable.Range(0, par.Length).Select(i => typeof(object)).ToArray());

        new AstReturn { ReturnType = typeof(object), ReturnValue = CreateCallMethod(mi, par) }.Compile(
            new CompilationContext(methodBuilder.GetILGenerator()));

        return tb.CreateType();
    }

    private static Type BuildActionCallerType(string typeName, MethodInfo mi)
    {
        var par = mi.GetParameters();
        Type actionCallerType = par.Length switch
        {
            0 => typeof(MethodInvokerAction0),
            1 => typeof(MethodInvokerAction1),
            2 => typeof(MethodInvokerAction2),
            3 => typeof(MethodInvokerAction3),
            _ => throw new EmitMapperException("too many method parameters")
        };

        var tb = DynamicAssemblyManager.DefineType(typeName, actionCallerType);

        var methodBuilder = tb.DefineMethod(
            "CallAction",
            MethodAttributes.Public | MethodAttributes.Virtual,
            null,
            Enumerable.Range(0, par.Length).Select(i => typeof(object)).ToArray());

        new AstComplexNode { Nodes = new List<IAstNode> { CreateCallMethod(mi, par), new AstReturnVoid() } }.Compile(
            new CompilationContext(methodBuilder.GetILGenerator()));

        return tb.CreateType();
    }

    private static IAstRefOrValue CreateCallMethod(MethodInfo mi, ParameterInfo[] parameters)
    {
        return AstBuildHelper.CallMethod(
            mi,
            mi.IsStatic
                ? null
                : new AstCastclassRef(
                    AstBuildHelper.ReadFieldRV(
                        new AstReadThis { ThisType = typeof(MethodInvokerBase) },
                        typeof(MethodInvokerBase).GetField(
                            nameof(MethodInvokerBase.TargetObject),
                            BindingFlags.Public | BindingFlags.Instance)),
                    mi.DeclaringType),
            parameters.Select((p, idx) => (IAstStackItem)AstBuildHelper.ReadArgumentRV(idx + 1, typeof(object)))
                .ToList());
    }
}