namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast write property.
/// </summary>
internal class AstWriteProperty : IAstNode
{
	private readonly PropertyInfo propertyInfo;

	private readonly MethodInfo setMethod;

	private readonly IAstRefOrAddr targetObject;

	private readonly IAstRefOrValue value;

	/// <summary>
	///   Initializes a new instance of the <see cref="AstWriteProperty" /> class.
	/// </summary>
	/// <param name="targetObject">The target object.</param>
	/// <param name="value">The value.</param>
	/// <param name="propertyInfo">The property info.</param>
	public AstWriteProperty(IAstRefOrAddr targetObject, IAstRefOrValue value, PropertyInfo propertyInfo)
	{
		this.targetObject = targetObject;
		this.value = value;
		this.propertyInfo = propertyInfo;
		setMethod = propertyInfo.GetSetMethod();

		switch (setMethod)
		{
			case null:
				throw new Exception("Property " + propertyInfo.Name + " doesn't have set accessor");
		}

		if (setMethod.GetParameters().Length != 1)
		{
			throw new EmitMapperException("Property " + propertyInfo.Name + " has invalid arguments");
		}
	}

	/// <inheritdoc />
	public void Compile(CompilationContext context)
	{
		AstBuildHelper.ICallMethod(setMethod, targetObject, new List<IAstStackItem> { value }).Compile(context);
	}
}