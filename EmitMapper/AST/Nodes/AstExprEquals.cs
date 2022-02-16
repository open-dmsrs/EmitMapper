namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection.Emit;

using EmitMapper.AST.Interfaces;
using EmitMapper.Utils;

internal class AstExprEquals : IAstValue
{
  private readonly IAstValue _leftValue;

  private readonly IAstValue _rightValue;

  public AstExprEquals(IAstValue leftValue, IAstValue rightValue)
  {
    _leftValue = leftValue;
    _rightValue = rightValue;
  }

  public Type ItemType => Metadata<int>.Type;

  public void Compile(CompilationContext context)
  {
    _leftValue.Compile(context);
    _rightValue.Compile(context);
    context.Emit(OpCodes.Ceq);
  }
}