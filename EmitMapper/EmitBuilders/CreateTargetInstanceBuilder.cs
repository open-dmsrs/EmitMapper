using System;
using System.Reflection;
using System.Reflection.Emit;
using EmitMapper.AST;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using EmitMapper.AST.Nodes;
using EmitMapper.Utils;

namespace EmitMapper.EmitBuilders;

internal class CreateTargetInstanceBuilder
{
    public static void BuildCreateTargetInstanceMethod(Type type, TypeBuilder typeBuilder)
    {
        if (ReflectionUtils.IsNullable(type))
            type = Nullable.GetUnderlyingType(type);

        var methodBuilder = typeBuilder.DefineMethod(
            "CreateTargetInstance",
            MethodAttributes.Public | MethodAttributes.Virtual,
            typeof(object),
            null);

        var ilGen = methodBuilder.GetILGenerator();
        var context = new CompilationContext(ilGen);
        IAstRefOrValue returnValue;

        if (type.IsValueType)
        {
            var lb = ilGen.DeclareLocal(type);
            new AstInitializeLocalVariable(lb).Compile(context);

            returnValue = new AstBox { Value = AstBuildHelper.ReadLocalRV(lb) };
        }
        else
        {
            returnValue = ReflectionUtils.HasDefaultConstructor(type)
                ? new AstNewObject { ObjectType = type }
                : new AstConstantNull();
        }

        new AstReturn { ReturnType = type, ReturnValue = returnValue }.Compile(context);
    }
}