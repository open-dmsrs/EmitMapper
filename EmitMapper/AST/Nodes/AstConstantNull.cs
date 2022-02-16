namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection.Emit;

using EmitMapper.AST.Interfaces;
using EmitMapper.Utils;

internal class AstConstantNull : IAstRefOrValue
{
  public Type ItemType => Metadata<object>.Type;

  public void Compile(CompilationContext context)
  {
    context.Emit(OpCodes.Ldnull);
  }
}