using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

internal class AstTypeof : IAstRef
{
  public Type Type;

  #region IAstStackItem Members

  public Type ItemType => Meta<Type>.Type;

  #endregion

  #region IAstNode Members

  public void Compile(CompilationContext context)
  {
    context.Emit(OpCodes.Ldtoken, Type);
    context.EmitCall(OpCodes.Call, Meta<Type>.Type.GetMethod(nameof(Type.GetTypeFromHandle)));
  }

  #endregion
}