namespace EmitMapper.AST.Nodes;

/// <summary>
/// The ast call method.
/// </summary>
internal class AstCallMethod : IAstRefOrValue
{
	/// <summary>
	/// Arguments
	/// </summary>
	public List<IAstStackItem> Arguments;

	/// <summary>
	/// Invocation object
	/// </summary>
	public IAstRefOrAddr InvocationObject;

	/// <summary>
	/// Method info
	/// </summary>
	public MethodInfo MethodInfo;

	/// <summary>
	/// Initializes a new instance of the <see cref="AstCallMethod"/> class.
	/// </summary>
	/// <param name="methodInfo">The method info.</param>
	/// <param name="invocationObject">The invocation object.</param>
	/// <param name="arguments">The arguments.</param>
	public AstCallMethod(MethodInfo methodInfo, IAstRefOrAddr invocationObject, List<IAstStackItem> arguments)
	{
		if (methodInfo is null)
		{
			throw new InvalidOperationException("methodInfo is null");
		}

		MethodInfo = methodInfo;
		InvocationObject = invocationObject;
		Arguments = arguments;
	}

	/// <summary>
	/// Gets the item type.
	/// </summary>
	public Type ItemType => MethodInfo.ReturnType;

	/// <inheritdoc/>
	public virtual void Compile(CompilationContext context)
	{
		CompilationHelper.EmitCall(context, InvocationObject, MethodInfo, Arguments);
	}
}