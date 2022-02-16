namespace EmitMapper.AST.Nodes;

using System;
using System.Reflection.Emit;

using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

internal class AstReturn : IAstAddr
{
  public Type ReturnType;

  public IAstRefOrValue ReturnValue;

  public Type ItemType => ReturnType;

  public void Compile(CompilationContext context)
  {
    ReturnValue.Compile(context);
    CompilationHelper.PrepareValueOnStack(context, ReturnType, ReturnValue.ItemType);
    context.Emit(OpCodes.Ret);
  }
}