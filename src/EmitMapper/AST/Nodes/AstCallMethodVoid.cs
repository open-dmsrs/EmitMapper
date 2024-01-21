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
		this.MethodInfo = methodInfo;
		this.InvocationObject = invocationObject;
		this.Arguments = arguments;
	}

	/// <inheritdoc/>
	public void Compile(CompilationContext context)
	{
		new AstCallMethod(MethodInfo, InvocationObject, Arguments).Compile(context);

		if (MethodInfo.ReturnType != Metadata.Void)
		{
			context.Emit(OpCodes.Pop);
		}
	}
}