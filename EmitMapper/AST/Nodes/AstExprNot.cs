using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;
using EmitMapper.Utils;

namespace EmitMapper.AST.Nodes;

internal class AstExprNot : IAstValue
{
  private readonly IAstRefOrValue _value;

  public AstExprNot(IAstRefOrValue value)
  {
    _value = value;
  }

  public Type ItemType => Metadata<int>.Type;

  public void Compile(CompilationContext context)
  {
    context.Emit(OpCodes.Ldc_I4_0);
    _value.Compile(context);
    context.Emit(OpCodes.Ceq);
  }
}