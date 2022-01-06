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
                if (mi.ReturnType == TypeHome.Void)
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
            0 => Meta<MethodInvokerFunc0>.Type,
            1 => Meta<MethodInvokerFunc1>.Type,
            2 => Meta<MethodInvokerFunc2>.Type,
            3 => Meta<MethodInvokerFunc3>.Type,
            _ => throw new EmitMapperException("too many method parameters")
        };

        var tb = DynamicAssemblyManager.DefineType(typeName, funcCallerType);

        var methodBuilder = tb.DefineMethod(
            "CallFunc",
            MethodAttributes.Public | MethodAttributes.Virtual,
            Meta<object>.Type,
            Enumerable.Range(0, par.Length).Select(i => Meta<object>.Type).ToArray());

        new AstReturn { ReturnType = Meta<object>.Type, ReturnValue = CreateCallMethod(mi, par) }.Compile(
            new CompilationContext(methodBuilder.GetILGenerator()));

        return tb.CreateType();
    }

    private static Type BuildActionCallerType(string typeName, MethodInfo mi)
    {
        var par = mi.GetParameters();
        Type actionCallerType = par.Length switch
        {
            0 => Meta<MethodInvokerAction0>.Type,
            1 => Meta<MethodInvokerAction1>.Type,
            2 => Meta<MethodInvokerAction2>.Type,
            3 => Meta<MethodInvokerAction3>.Type,
            _ => throw new EmitMapperException("too many method parameters")
        };

        var tb = DynamicAssemblyManager.DefineType(typeName, actionCallerType);

        var methodBuilder = tb.DefineMethod(
            "CallAction",
            MethodAttributes.Public | MethodAttributes.Virtual,
            null,
            Enumerable.Range(0, par.Length).Select(i => Meta<object>.Type).ToArray());

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
                        new AstReadThis { ThisType = Meta<MethodInvokerBase>.Type },
                        Meta<MethodInvokerBase>.Type.GetField(
                            nameof(MethodInvokerBase.TargetObject),
                            BindingFlags.Public | BindingFlags.Instance)),
                    mi.DeclaringType),
            parameters.Select((p, idx) => (IAstStackItem)AstBuildHelper.ReadArgumentRV(idx + 1, Meta<object>.Type))
                .ToList());
    }
}