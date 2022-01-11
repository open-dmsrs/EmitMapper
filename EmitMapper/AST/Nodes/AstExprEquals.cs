using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;
using EmitMapper.Utils;

namespace EmitMapper.AST.Nodes;

internal class AstExprEquals : IAstValue
{
  private readonly IAstValue _leftValue;

  private readonly IAstValue _rightValue;

  public AstExprEquals(IAstValue leftValue, IAstValue rightValue)
  {
    _leftValue = leftValue;
    _rightValue = rightValue;
  }

  #region IAstReturnValueNode Members

  public Type ItemType => Metadata<int>.Type;

  #endregion

  #region IAstNode Members

  public void Compile(CompilationContext context)
  {
    _leftValue.Compile(context);
    _rightValue.Compile(context);
    context.Emit(OpCodes.Ceq);
  }

  #endregion
}