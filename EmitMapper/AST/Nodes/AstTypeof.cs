using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;
using EmitMapper.Utils;

namespace EmitMapper.AST.Nodes;

internal class AstTypeof : IAstRef
{
  public Type Type;

  #region IAstStackItem Members

  public Type ItemType => Metadata<Type>.Type;

  #endregion

  #region IAstNode Members

  public void Compile(CompilationContext context)
  {
    context.Emit(OpCodes.Ldtoken, Type);
    context.EmitCall(OpCodes.Call, Metadata<Type>.Type.GetMethodCache(nameof(Type.GetTypeFromHandle)));
  }

  #endregion
}