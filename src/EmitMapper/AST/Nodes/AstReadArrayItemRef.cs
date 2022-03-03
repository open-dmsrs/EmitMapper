using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast read array item ref.
/// </summary>
internal class AstReadArrayItemRef : AstReadArrayItem, IAstRef
{
  /// <summary>
  /// </summary>
  /// <param name="context">The context.</param>
  public override void Compile(CompilationContext context)
  {
    CompilationHelper.CheckIsRef(ItemType);
    base.Compile(context);
  }
}