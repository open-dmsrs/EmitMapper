namespace EmitMapper.AST.Helpers;

/// <summary>
/// The compilation helper.
/// </summary>
internal static class CompilationHelper
{
	/// <summary>
	/// Checks the is ref.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <exception cref="IlCompilationException"></exception>
	public static void CheckIsRef(Type? type)
	{
		if (type.IsValueType)
		{
			throw new IlCompilationException("A reference type was expected, but it was: " + type);
		}
	}

	/// <summary>
	/// Checks the is value.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <exception cref="IlCompilationException"></exception>
	public static void CheckIsValue(Type? type)
	{
		if (!type.IsValueType)
		{
			throw new IlCompilationException("A value type was expected, but it was: " + type);
		}
	}

	/// <summary>
	/// Emits the call.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <param name="invocationObject">The invocation object.</param>
	/// <param name="methodInfo">The method info.</param>
	/// <param name="arguments">The arguments.</param>
	/// <exception cref="Exception"></exception>
	public static void EmitCall(
	  CompilationContext context,
	  IAstRefOrAddr? invocationObject,
	  MethodInfo methodInfo,
	  List<IAstStackItem>? arguments)
	{
		arguments ??= new List<IAstStackItem>();

		invocationObject?.Compile(context);

		var args = methodInfo.GetParameters();

		if (args.Length != arguments.Count)
		{
			throw new ArgumentException("Invalid method parameters count");
		}

		for (var i = 0; i < args.Length; ++i)
		{
			arguments[i].Compile(context);
			PrepareValueOnStack(context, args[i].ParameterType, arguments[i].ItemType);
		}

		context.EmitCall(methodInfo.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, methodInfo);
	}

	/// <summary>
	/// Prepares the value on stack.
	/// </summary>
	/// <param name="context">The context.</param>
	/// <param name="desiredType">The desired type.</param>
	/// <param name="typeOnStack">The type on stack.</param>
	public static void PrepareValueOnStack(CompilationContext context, Type? desiredType, Type? typeOnStack)
	{
		if (typeOnStack.IsValueType && !desiredType.IsValueType)
		{
			context.Emit(OpCodes.Box, typeOnStack);
		}
		else if (!typeOnStack.IsValueType && desiredType.IsValueType)
		{
			context.Emit(OpCodes.Unbox_Any, desiredType);
		}
		else if (desiredType != typeOnStack)
		{
			context.Emit(OpCodes.Castclass, desiredType);
		}
	}
}