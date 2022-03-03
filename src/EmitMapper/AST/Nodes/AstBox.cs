using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

internal class AstBox : IAstRef
{
  public IAstRefOrValue Value;

  public Type ItemType => Value.ItemType;

  public void Compile(CompilationContext context)
  {
    Value.Compile(context);

    if (Value.ItemType.IsValueType)
      context.Emit(OpCodes.Box, ItemType);
  }
}