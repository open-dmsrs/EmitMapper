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
                if (del.Method.ReturnType == typeof(void))
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
        Type funcCallerType = par.Length switch
        {
            0 => typeof(DelegateInvokerFunc0),
            1 => typeof(DelegateInvokerFunc1),
            2 => typeof(DelegateInvokerFunc2),
            3 => typeof(DelegateInvokerFunc3),
            _ => throw new EmitMapperException("too many method parameters")
        };

        var tb = DynamicAssemblyManager.DefineType(typeName, funcCallerType);

        var methodBuilder = tb.DefineMethod(
            "CallFunc",
            MethodAttributes.Public | MethodAttributes.Virtual,
            typeof(object),
            Enumerable.Range(0, par.Length).Select(i => typeof(object)).ToArray());

        new AstReturn { ReturnType = typeof(object), ReturnValue = CreateCallDelegate(del, par) }.Compile(
            new CompilationContext(methodBuilder.GetILGenerator()));

        return tb.CreateType();
    }

    private static Type BuildActionCallerType(string typeName, Delegate del)
    {
        var par = del.Method.GetParameters();
        Type actionCallerType = par.Length switch
        {
            0 => typeof(DelegateInvokerAction0),
            1 => typeof(DelegateInvokerAction1),
            2 => typeof(DelegateInvokerAction2),
            3 => typeof(DelegateInvokerAction3),
            _ => throw new EmitMapperException("too many method parameters")
        };

        var tb = DynamicAssemblyManager.DefineType(typeName, actionCallerType);

        var methodBuilder = tb.DefineMethod(
            "CallAction",
            MethodAttributes.Public | MethodAttributes.Virtual,
            null,
            Enumerable.Range(0, par.Length).Select(i => typeof(object)).ToArray());

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
                    new AstReadThis { ThisType = typeof(DelegateInvokerBase) },
                    typeof(DelegateInvokerBase).GetField(
                        nameof(DelegateInvokerBase.Del),
                        BindingFlags.Public | BindingFlags.Instance)),
                del.GetType()),
            parameters.Select((p, idx) => (IAstStackItem)AstBuildHelper.ReadArgumentRV(idx + 1, typeof(object)))
                .ToList());
    }
}