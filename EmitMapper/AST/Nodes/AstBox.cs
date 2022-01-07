using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

internal class AstBox : IAstRef
{
  public IAstRefOrValue Value;

  #region IAstReturnValueNode Members

  public Type ItemType => Value.ItemType;

  #endregion

  #region IAstNode Members

  public void Compile(CompilationContext context)
  {
    Value.Compile(context);

    if (Value.ItemType.IsValueType)
      context.Emit(OpCodes.Box, ItemType);
  }

  #endregion
}