using System.Collections.Generic;
using System.Reflection;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast call method ref.
/// </summary>
internal class AstCallMethodRef : AstCallMethod, IAstRef
{
  /// <summary>
  ///   Initializes a new instance of the <see cref="AstCallMethodRef" /> class.
  /// </summary>
  /// <param name="methodInfo">The method info.</param>
  /// <param name="invocationObject">The invocation object.</param>
  /// <param name="arguments">The arguments.</param>
  public AstCallMethodRef(MethodInfo methodInfo, IAstRefOrAddr invocationObject, List<IAstStackItem> arguments)
    : base(methodInfo, invocationObject, arguments)
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