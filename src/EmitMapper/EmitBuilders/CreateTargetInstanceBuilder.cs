namespace EmitMapper.EmitBuilders;

/// <summary>
///   The create target instance builder.
/// </summary>
internal static class CreateTargetInstanceBuilder
{
	/// <summary>
	///   Builds the create target instance method.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="typeBuilder">The type builder.</param>
	public static void BuildCreateTargetInstanceMethod(Type type, TypeBuilder typeBuilder)
	{
		// var expr = (Expression<Func<object>>)ObjectFactory.GenerateConstructorExpression(type).ToObject();
		if (ReflectionHelper.IsNullable(type))
		{
			type = type.GetUnderlyingTypeCache();
		}

		var methodBuilder = typeBuilder.DefineMethod(
		  nameof(MapperBase.CreateTargetInstance),
		  MethodAttributes.Public | MethodAttributes.Virtual,
		  Metadata<object>.Type,
		  null);

		var ilGen = methodBuilder.GetILGenerator();
		var context = new CompilationContext(ilGen);
		IAstRefOrValue returnValue;

		if (type.IsValueType)
		{
			var lb = ilGen.DeclareLocal(type);
			new AstInitializeLocalVariable(lb).Compile(context);

			returnValue = new AstBox { Value = AstBuildHelper.ReadLocalRV(lb) };
		}
		else
		{
			returnValue = ReflectionHelper.HasDefaultConstructor(type)
			  ? new AstNewObject { ObjectType = type }
			  : new AstConstantNull();
		}

		new AstReturn { ReturnType = type, ReturnValue = returnValue }.Compile(context);
	}
}