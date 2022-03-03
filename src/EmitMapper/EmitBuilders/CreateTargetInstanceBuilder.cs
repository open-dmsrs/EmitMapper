using System;
using System.Reflection;
using System.Reflection.Emit;
using EmitMapper.AST;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using EmitMapper.AST.Nodes;
using EmitMapper.Mappers;
using EmitMapper.Utils;

namespace EmitMapper.EmitBuilders;

internal static class CreateTargetInstanceBuilder
{
  public static void BuildCreateTargetInstanceMethod(Type type, TypeBuilder typeBuilder)
  {
    // var expr = (Expression<Func<object>>)ObjectFactory.GenerateConstructorExpression(type).ToObject();
    if (ReflectionHelper.IsNullable(type))
      type = type.GetUnderlyingTypeCache();

    var methodBuilder = typeBuilder.DefineMethod(
      nameof(MapperBase.CreateTargetInstance),
      MethodAttributes.Public | MethodAttributes.Virtual,
      Metadata<object>.Type,
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
      returnValue = ReflectionHelper.HasDefaultConstructor(type)
        ? new AstNewObject { ObjectType = type }
        : new AstConstantNull();
    }

    new AstReturn { ReturnType = type, ReturnValue = returnValue }.Compile(context);
  }
}