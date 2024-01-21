namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast exception handling block.
/// </summary>
internal class AstExceptionHandlingBlock : IAstNode
{
	private readonly Type? _exceptionType;

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
	  Type? exceptionType,
	  LocalBuilder exceptionVariable)
	{
		this._protectedBlock = protectedBlock;
		this._handlerBlock = handlerBlock;
		this._exceptionType = exceptionType;
		this._exceptionVariable = exceptionVariable;
	}

	/// <inheritdoc />
	public void Compile(CompilationContext context)
	{
		context.IlGenerator.BeginExceptionBlock();
		_protectedBlock.Compile(context);
		context.IlGenerator.BeginCatchBlock(_exceptionType);
		context.IlGenerator.Emit(OpCodes.Stloc, _exceptionVariable);
		_handlerBlock.Compile(context);
		context.IlGenerator.EndExceptionBlock();
	}
}