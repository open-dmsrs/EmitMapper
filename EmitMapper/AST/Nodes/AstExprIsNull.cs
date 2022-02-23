using System;
using System.Reflection.Emit;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using EmitMapper.Utils;

namespace EmitMapper.AST.Nodes;

internal class AstExprIsNull : IAstValue
{
  private readonly IAstRefOrValue _value;

  public AstExprIsNull(IAstRefOrValue value)
  {
    _value = value;
  }

  public Type ItemType => Metadata<int>.Type;

  public void Compile(CompilationContext context)
  {
    if (!(_value is IAstRef) && !ReflectionHelper.IsNullable(_value.ItemType))
    {
      context.Emit(OpCodes.Ldc_I4_1);
    }
    else if (ReflectionHelper.IsNullable(_value.ItemType))
    {
      AstBuildHelper.ReadPropertyRV(new AstValueToAddr((IAstValue)_value), _value.ItemType.GetProperty("HasValue"))
        .Compile(context);

      context.Emit(OpCodes.Ldc_I4_0);
      context.Emit(OpCodes.Ceq);
    }
    else
    {
      _value.Compile(context);
      new AstConstantNull().Compile(context);
      context.Emit(OpCodes.Ceq);
    }
  }
}