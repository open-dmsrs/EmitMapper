namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast exception handling block.
/// </summary>
internal class AstExceptionHandlingBlock : IAstNode
{
  private readonly Type _exceptionType;

  private readonly LocalBuilder _exceptionVariable;

  private readonly IAstNode _handlerBlock;

  private readonly IAstNode _protectedBlock;

  /// <summary>
  ///   Initializes a new instance of the <see cref="AstExceptionHandlingBlock" /> class.
  /// </summary>
  /// <param name="protectedBlock">The protected block.</param>
  /// <param name="handlerBlock">The handler block.</param>
  /// <param name="exceptionType">The exception type.</param>
  /// <param name="exceptionVariable">The exception variable.</param>
  public AstExceptionHandlingBlock(
    IAstNode protectedBlock,
    IAstNode handlerBlock,
    Type exceptionType,
    LocalBuilder exceptionVariable)
  {
    _protectedBlock = protectedBlock;
    _handlerBlock = handlerBlock;
    _exceptionType = exceptionType;
    _exceptionVariable = exceptionVariable;
  }

/// <inheritdoc />
  public void Compile(CompilationContext context)
  {
    context.ILGenerator.BeginExceptionBlock();
    _protectedBlock.Compile(context);
    context.ILGenerator.BeginCatchBlock(_exceptionType);
    context.ILGenerator.Emit(OpCodes.Stloc, _exceptionVariable);
    _handlerBlock.Compile(context);
    context.ILGenerator.EndExceptionBlock();
  }
}