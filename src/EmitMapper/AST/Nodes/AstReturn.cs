using System;
using System.Reflection.Emit;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;
/// <summary>
/// The ast return.
/// </summary>

internal class AstReturn : IAstAddr
{
  public Type ReturnType;

  public IAstRefOrValue ReturnValue;

  /// <summary>
  /// Gets the item type.
  /// </summary>
  public Type ItemType => ReturnType;

  /// <summary>
  /// 
  /// </summary>
  /// <param name="context">The context.</param>
  public void Compile(CompilationContext context)
  {
    ReturnValue.Compile(context);
    CompilationHelper.PrepareValueOnStack(context, ReturnType, ReturnValue.ItemType);
    context.Emit(OpCodes.Ret);
  }
}