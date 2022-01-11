using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;
using EmitMapper.Utils;

namespace EmitMapper.AST.Nodes;

internal class AstConstantString : IAstRef
{
  public string Str;

  #region IAstStackItem Members

  public Type ItemType => Metadata<string>.Type;

  #endregion

  #region IAstNode Members

  public void Compile(CompilationContext context)
  {
    context.Emit(OpCodes.Ldstr, Str);
  }

  #endregion
}