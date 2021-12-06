using EmitMapper.AST;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using EmitMapper.AST.Nodes;
using EmitMapper.Utils;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace EmitMapper.EmitBuilders
{
    internal class CreateTargetInstanceBuilder
    {
        public static void BuildCreateTargetInstanceMethod(Type type, TypeBuilder typeBuilder)
        {
            if (ReflectionUtils.IsNullable(type))
            {
                type = Nullable.GetUnderlyingType(type);
            }

            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                "CreateTargetInstance",
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(object),
                null
                );

            ILGenerator ilGen = methodBuilder.GetILGenerator();
            CompilationContext context = new CompilationContext(ilGen);
            IAstRefOrValue returnValue;

            if (type.IsValueType)
            {
                LocalBuilder lb = ilGen.DeclareLocal(type);
                new AstInitializeLocalVariable(lb).Compile(context);

                returnValue =
                    new AstBox()
                    {
                        Value = AstBuildHelper.ReadLocalRV(lb)
                    };
            }
            else
            {
                returnValue =
                    ReflectionUtils.HasDefaultConstructor(type)
                        ? new AstNewObject() { ObjectType = type } : new AstConstantNull();
            }
            new AstReturn()
            {
                ReturnType = type,
                ReturnValue = returnValue
            }.Compile(context);
        }
    }
}