using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;
using EmitMapper.Utils;

namespace EmitMapper.AST.Nodes;
/// <summary>
/// The ast call method void.
/// </summary>

internal class AstCallMethodVoid : IAstNode
{
  protected List<IAstStackItem> Arguments;

  protected IAstRefOrAddr InvocationObject;

  protected MethodInfo MethodInfo;

  /// <summary>
  /// Initializes a new instance of the <see cref="AstCallMethodVoid"/> class.
  /// </summary>
  /// <param name="methodInfo">The method info.</param>
  /// <param name="invocationObject">The invocation object.</param>
  /// <param name="arguments">The arguments.</param>
  public AstCallMethodVoid(MethodInfo methodInfo, IAstRefOrAddr invocationObject, List<IAstStackItem> arguments)
  {
    MethodInfo = methodInfo;
    InvocationObject = invocationObject;
    Arguments = arguments;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="context">The context.</param>
  public void Compile(CompilationContext context)
  {
    new AstCallMethod(MethodInfo, InvocationObject, Arguments).Compile(context);

    if (MethodInfo.ReturnType != Metadata.Void)
      context.Emit(OpCodes.Pop);
  }
}