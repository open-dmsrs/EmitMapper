namespace EmitMapper.AST.Nodes;

/// <summary>
/// The ast new object.
/// </summary>
internal class AstNewObject : IAstRef
{
	public IAstStackItem[] ConstructorParams;

	public Type ObjectType;

	/// <summary>
	/// Initializes a new instance of the <see cref="AstNewObject"/> class.
	/// </summary>
	public AstNewObject()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="AstNewObject"/> class.
	/// </summary>
	/// <param name="objectType">The object type.</param>
	/// <param name="constructorParams">The constructor params.</param>
	public AstNewObject(Type objectType, IAstStackItem[] constructorParams)
	{
		ObjectType = objectType;
		ConstructorParams = constructorParams;
	}

	/// <summary>
	/// Gets the item type.
	/// </summary>
	public Type ItemType => ObjectType;

	/// <inheritdoc/>
	/// <exception cref="Exception"></exception>
	public void Compile(CompilationContext context)
	{
		if (ReflectionHelper.IsNullable(ObjectType))
		{
			IAstRefOrValue underlyingValue;
			var underlyingType = ObjectType.GetUnderlyingTypeCache();

			if (ConstructorParams is null || ConstructorParams.Length == 0)
			{
				var temp = context.ILGenerator.DeclareLocal(underlyingType);
				new AstInitializeLocalVariable(temp).Compile(context);
				underlyingValue = AstBuildHelper.ReadLocalRV(temp);
			}
			else
			{
				underlyingValue = (IAstValue)ConstructorParams[0];
			}

			var constructor = ObjectType.GetConstructor(
			  BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance,
			  null,
			  new[] { underlyingType },
			  null);

			underlyingValue.Compile(context);
			context.EmitNewObject(constructor);
		}
		else
		{
			IEnumerable<Type> types = Type.EmptyTypes;

			if (ConstructorParams?.Length > 0)
			{
				types = ConstructorParams.Select(c => c.ItemType);

				foreach (var p in ConstructorParams)
				{
					p.Compile(context);
				}
			}

			var ci = ObjectType.GetConstructor(types.ToArray());

			if (ci is not null)
			{
				context.EmitNewObject(ci);
			}
			else if (ObjectType.IsValueType)
			{
				var temp = context.ILGenerator.DeclareLocal(ObjectType);
				new AstInitializeLocalVariable(temp).Compile(context);
				AstBuildHelper.ReadLocalRV(temp).Compile(context);
			}
			else
			{
				throw new NotImplementedException($"Constructor for types [{types.ToCsv(",")}] not found in {ObjectType.FullName}");
			}
		}
	}
}