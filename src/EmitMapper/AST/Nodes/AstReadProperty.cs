namespace EmitMapper.AST.Nodes;

/// <summary>
///   The ast read property.
/// </summary>
internal class AstReadProperty : IAstRefOrValue
{
	/// <summary>
	/// Property info
	/// </summary>
	public PropertyInfo? PropertyInfo;

	/// <summary>
	/// Source object
	/// </summary>
	public IAstRefOrAddr? SourceObject;

	/// <summary>
	///   Gets the item type.
	/// </summary>
	public Type ItemType => PropertyInfo.PropertyType;

	/// <inheritdoc />
	/// <exception cref="Exception"></exception>
	public virtual void Compile(CompilationContext context)
	{
		var mi = PropertyInfo.GetGetMethod();

		switch (mi)
		{
			case null:
				throw new Exception("Property " + PropertyInfo.Name + " doesn't have get accessor");
			default:
				AstBuildHelper.CallMethod(mi, SourceObject, null).Compile(context);

				break;
		}
	}
}