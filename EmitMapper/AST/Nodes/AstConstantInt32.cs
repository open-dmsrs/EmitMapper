using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

internal class AstConstantInt32 : IAstValue
{
  public int Value;

  #region IAstReturnValueNode Members

  public Type ItemType => Meta<int>.Type;

  #endregion

  #region IAstNode Members

  public void Compile(CompilationContext context)
  {
    context.Emit(OpCodes.Ldc_I4, Value);
  }

  #endregion
}