using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;
using EmitMapper.Utils;

namespace EmitMapper.AST.Nodes;

internal class AstConstantNull : IAstRefOrValue
{
  #region IAstReturnValueNode Members

  public Type ItemType => Metadata<object>.Type;

  #endregion

  #region IAstNode Members

  public void Compile(CompilationContext context)
  {
    context.Emit(OpCodes.Ldnull);
  }

  #endregion
}