namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast exception handling block.
/// </summary>
internal class AstExceptionHandlingBlock : IAstNode
{
	private readonly Type exceptionType;

	private readonly LocalBuilder exceptionVariable;

	private readonly IAstNode handlerBlock;

	private readonly IAstNode protectedBlock;

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
		this.protectedBlock = protectedBlock;
		this.handlerBlock = handlerBlock;
		this.exceptionType = exceptionType;
		this.exceptionVariable = exceptionVariable;
	}

	/// <inheritdoc />
	public void Compile(CompilationContext context)
	{
		context.ILGenerator.BeginExceptionBlock();
		protectedBlock.Compile(context);
		context.ILGenerator.BeginCatchBlock(exceptionType);
		context.ILGenerator.Emit(OpCodes.Stloc, exceptionVariable);
		handlerBlock.Compile(context);
		context.ILGenerator.EndExceptionBlock();
	}
}