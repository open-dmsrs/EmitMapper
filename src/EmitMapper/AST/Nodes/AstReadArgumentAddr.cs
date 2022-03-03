using System.Reflection.Emit;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;
/// <summary>
/// The ast read argument addr.
/// </summary>

internal class AstReadArgumentAddr : AstReadArgument, IAstAddr
{
  /// <summary>
  /// 
  /// </summary>
  /// <param name="context">The context.</param>
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsValue(ItemType);
    context.Emit(OpCodes.Ldarga, ArgumentIndex);
  }
}