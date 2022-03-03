using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast if ternar.
/// </summary>
internal class AstIfTernar : IAstRefOrValue
{
  public IAstRefOrValue Condition;

  public IAstRefOrValue FalseBranch;

  public IAstRefOrValue TrueBranch;

  /// <summary>
  ///   Initializes a new instance of the <see cref="AstIfTernar" /> class.
  /// </summary>
  /// <param name="condition">The condition.</param>
  /// <param name="trueBranch">The true branch.</param>
  /// <param name="falseBranch">The false branch.</param>
  public AstIfTernar(IAstRefOrValue condition, IAstRefOrValue trueBranch, IAstRefOrValue falseBranch)
  {
    if (trueBranch.ItemType != falseBranch.ItemType)
      throw new EmitMapperException("Types mismatch");

    Condition = condition;
    TrueBranch = trueBranch;
    FalseBranch = falseBranch;
  }

  /// <summary>
  ///   Gets the item type.
  /// </summary>
  public Type ItemType => TrueBranch.ItemType;

  /// <summary>
  /// </summary>
  /// <param name="context">The context.</param>
  public void Compile(CompilationContext context)
  {
    var elseLabel = context.ILGenerator.DefineLabel();
    var endIfLabel = context.ILGenerator.DefineLabel();

    Condition.Compile(context);
    context.Emit(OpCodes.Brfalse, elseLabel);

    if (TrueBranch != null)
      TrueBranch.Compile(context);

    if (FalseBranch != null)
      context.Emit(OpCodes.Br, endIfLabel);

    context.ILGenerator.MarkLabel(elseLabel);

    if (FalseBranch != null)
      FalseBranch.Compile(context);

    context.ILGenerator.MarkLabel(endIfLabel);
  }
}