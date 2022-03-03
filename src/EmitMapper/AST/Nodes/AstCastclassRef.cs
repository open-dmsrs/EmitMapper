using System;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast castclass ref.
/// </summary>
internal class AstCastclassRef : AstCastclass, IAstRef
{
  /// <summary>
  ///   Initializes a new instance of the <see cref="AstCastclassRef" /> class.
  /// </summary>
  /// <param name="value">The value.</param>
  /// <param name="targetType">The target type.</param>
  public AstCastclassRef(IAstRefOrValue value, Type targetType)
    : base(value, targetType)
  {
  }

  /// <summary>
  /// </summary>
  /// <param name="context">The context.</param>
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsRef(ItemType);
    base.Compile(context);
  }
}