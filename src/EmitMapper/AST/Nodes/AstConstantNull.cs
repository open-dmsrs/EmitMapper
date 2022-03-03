using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;
using EmitMapper.Utils;

namespace EmitMapper.AST.Nodes;

internal class AstConstantNull : IAstRefOrValue
{
  public Type ItemType => Metadata<object>.Type;

  public void Compile(CompilationContext context)
  {
    context.Emit(OpCodes.Ldnull);
  }
}