using System;
using System.Reflection.Emit;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using EmitMapper.Utils;

namespace EmitMapper.AST.Nodes;

internal class AstIndirectReadValue : AstIndirectRead, IAstValue
{
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsValue(ItemType);

    if (ItemType == Metadata<int>.Type)
      context.Emit(OpCodes.Ldind_I4);
    else
      throw new Exception("Unsupported type");
  }
}