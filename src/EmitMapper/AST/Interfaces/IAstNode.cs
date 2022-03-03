namespace EmitMapper.AST.Interfaces;
/// <summary>
/// The ast node interface.
/// </summary>

internal interface IAstNode
{
  /// <summary>
  /// 
  /// </summary>
  /// <param name="context">The context.</param>
  void Compile(CompilationContext context);
}