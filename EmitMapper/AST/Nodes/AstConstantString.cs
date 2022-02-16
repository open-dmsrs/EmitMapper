namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection.Emit;

using EmitMapper.AST.Interfaces;
using EmitMapper.Utils;

internal class AstConstantString : IAstRef
{
  public string Str;

  public Type ItemType => Metadata<string>.Type;

  public void Compile(CompilationContext context)
  {
    context.Emit(OpCodes.Ldstr, Str);
  }
}