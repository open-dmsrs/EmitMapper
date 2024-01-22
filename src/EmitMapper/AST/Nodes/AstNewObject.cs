namespace EmitMapper.AST.Nodes;

/// <summary>
/// The ast new object.
/// </summary>
internal class AstNewObject : IAstRef
{
	/// <summary>
	/// Constructor params
	/// </summary>
	public IAstStackItem[] ConstructorParams;

	/// <summary>
	/// Object type
	/// </summary>
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
				var temp = context.IlGenerator.DeclareLocal(underlyingType);
				new AstInitializeLocalVariable(temp).Compile(context);
				underlyingValue = AstBuildHelper.IReadLocalRv(temp);
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

			switch (ConstructorParams?.Length)
			{
				case > 0:
				{
					types = ConstructorParams.Select(c => c.ItemType);

					foreach (var p in ConstructorParams)
					{
						p.Compile(context);
					}

					break;
				}
			}

			var ci = ObjectType.GetConstructor(types.ToArray());

			if (ci is not null)
			{
				context.EmitNewObject(ci);
			}
			else switch (ObjectType.IsValueType)
			{
				case true:
				{
					var temp = context.IlGenerator.DeclareLocal(ObjectType);
					new AstInitializeLocalVariable(temp).Compile(context);
					AstBuildHelper.IReadLocalRv(temp).Compile(context);

					break;
				}
				default:
					throw new NotImplementedException($"Constructor for types [{types.ToCsv(",")}] not found in {ObjectType.FullName}");
			}
		}
	}
}