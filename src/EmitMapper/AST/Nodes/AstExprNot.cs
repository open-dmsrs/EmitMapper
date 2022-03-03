using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;
using EmitMapper.Utils;

namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast expr not.
/// </summary>
internal class AstExprNot : IAstValue
{
  private readonly IAstRefOrValue _value;

  /// <summary>
  ///   Initializes a new instance of the <see cref="AstExprNot" /> class.
  /// </summary>
  /// <param name="value">The value.</param>
  public AstExprNot(IAstRefOrValue value)
  {
    _value = value;
  }

  /// <summary>
  ///   Gets the item type.
  /// </summary>
  public Type ItemType => Metadata<int>.Type;

  /// <summary>
  /// </summary>
  /// <param name="context">The context.</param>
  public void Compile(CompilationContext context)
  {
    context.Emit(OpCodes.Ldc_I4_0);
    _value.Compile(context);
    context.Emit(OpCodes.Ceq);
  }
}