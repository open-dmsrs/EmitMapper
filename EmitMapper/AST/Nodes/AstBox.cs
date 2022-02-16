namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection.Emit;

using EmitMapper.AST.Interfaces;

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