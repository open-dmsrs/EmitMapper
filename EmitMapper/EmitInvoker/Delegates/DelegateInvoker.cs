using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        var creator = ThreadSaveCache.GetCtor(
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
        Type funcCallerType = null;
        if (par.Length == 0)
            funcCallerType = typeof(DelegateInvokerFunc0);
        else if (par.Length == 1)
            funcCallerType = typeof(DelegateInvokerFunc1);
        else if (par.Length == 2)
            funcCallerType = typeof(DelegateInvokerFunc2);
        else if (par.Length == 3)
            funcCallerType = typeof(DelegateInvokerFunc3);
        else
            throw new EmitMapperException("too many method parameters");

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
        Type actionCallerType = null;
        if (par.Length == 0)
            actionCallerType = typeof(DelegateInvokerAction0);
        else if (par.Length == 1)
            actionCallerType = typeof(DelegateInvokerAction1);
        else if (par.Length == 2)
            actionCallerType = typeof(DelegateInvokerAction2);
        else if (par.Length == 3)
            actionCallerType = typeof(DelegateInvokerAction3);
        else
           throw new EmitMapperException("too many method parameters");

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