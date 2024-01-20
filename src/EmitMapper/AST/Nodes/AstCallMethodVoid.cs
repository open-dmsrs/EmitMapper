namespace EmitMapper.AST.Nodes;

/// <summary>
/// The ast call method void.
/// </summary>
internal class AstCallMethodVoid : IAstNode
{
	protected List<IAstStackItem> arguments;

	protected IAstRefOrAddr invocationObject;

	protected MethodInfo methodInfo;

	/// <summary>
	/// Initializes a new instance of the <see cref="AstCallMethodVoid"/> class.
	/// </summary>
	/// <param name="methodInfo">The method info.</param>
	/// <param name="invocationObject">The invocation object.</param>
	/// <param name="arguments">The arguments.</param>
	public AstCallMethodVoid(MethodInfo methodInfo, IAstRefOrAddr invocationObject, List<IAstStackItem> arguments)
	{
		this.methodInfo = methodInfo;
		this.invocationObject = invocationObject;
		this.arguments = arguments;
	}

	/// <inheritdoc/>
	public void Compile(CompilationContext context)
	{
		new AstCallMethod(methodInfo, invocationObject, arguments).Compile(context);

		if (methodInfo.ReturnType != Metadata.Void)
		{
			context.Emit(OpCodes.Pop);
		}
	}
}