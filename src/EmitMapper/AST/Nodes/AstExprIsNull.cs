using System;
using System.Reflection.Emit;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using EmitMapper.Utils;

namespace EmitMapper.AST.Nodes;
/// <summary>
/// The ast expr is null.
/// </summary>

internal class AstExprIsNull : IAstValue
{
  private readonly IAstRefOrValue _value;

  /// <summary>
  /// Initializes a new instance of the <see cref="AstExprIsNull"/> class.
  /// </summary>
  /// <param name="value">The value.</param>
  public AstExprIsNull(IAstRefOrValue value)
  {
    _value = value;
  }

  /// <summary>
  /// Gets the item type.
  /// </summary>
  public Type ItemType => Metadata<int>.Type;

  /// <summary>
  /// 
  /// </summary>
  /// <param name="context">The context.</param>
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