using System;
using System.Reflection.Emit;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

internal class AstWriteLocal : IAstNode
{
  public IAstRefOrValue Value;
  public int LocalIndex;

  public Type LocalType;

  public AstWriteLocal()
  {
  }

  public AstWriteLocal(LocalBuilder loc, IAstRefOrValue value)
  {
    LocalIndex = loc.LocalIndex;
    LocalType = loc.LocalType;
    Value = value;
  }

  public void Compile(CompilationContext context)
  {
    Value.Compile(context);
    CompilationHelper.PrepareValueOnStack(context, LocalType, Value.ItemType);
    context.Emit(OpCodes.Stloc, LocalIndex);
  }
}