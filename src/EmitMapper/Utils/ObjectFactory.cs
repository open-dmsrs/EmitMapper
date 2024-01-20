using System.Linq.Expressions;
using FastExpressionCompiler;

namespace EmitMapper.Utils;

using static Expression;
using static ExpressionHelper;

/// <summary>
/// The object factory.
/// </summary>
public static class ObjectFactory
{
	private static readonly LazyConcurrentDictionary<Type, Func<object>> CtorCache = new();

	/// <summary>
	/// Creates the instance.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="type">The type.</param>
	/// <returns>A <typeparamref name="T"></typeparamref></returns>
	public static T CreateInstance<T>(Type type)
	{
		return (T)CreateInstance(type);
	}

	/// <summary>
	/// Creates the instance.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>An object.</returns>
	public static object CreateInstance(Type type)
	{
		return CtorCache.GetOrAdd(type, GenerateConstructor)();
	}

	/// <summary>
	/// Creates the interface proxy.
	/// </summary>
	/// <param name="interfaceType">The interface type.</param>
	/// <returns>An object.</returns>
	public static object CreateInterfaceProxy(Type interfaceType)
	{
		return CreateInstance(ProxyGenerator.GetProxyType(interfaceType));
	}

	/// <summary>
	/// Generates the constructor expression.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>An Expression.</returns>
	public static Expression GenerateConstructorExpression(Type type)
	{
		return type switch
		{
			{ IsValueType: true } => Default(type),
			Type stringType when stringType == Metadata<string>.Type => Constant(string.Empty),
			{ IsInterface: true } => CreateInterfaceExpression(type),
			{ IsAbstract: true } => InvalidType(type, $"Cannot create an instance of abstract type {type}."),
			_ => CallConstructor(type)
		};
	}

	/// <summary>
	/// Calls the constructor.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>An Expression.</returns>
	private static Expression CallConstructor(Type type)
	{
		var defaultCtor = type.GetConstructor(TypeExtensions.InstanceFlags, null, Type.EmptyTypes, null);

		if (defaultCtor is not null)
		{
			return New(defaultCtor);
		}

		// find a ctor with only optional args
		var ctorWithOptionalArgs =
		  type.GetDeclaredConstructors().FirstOrDefault(c => c.GetParameters().All(p => p.IsOptional));

		if (ctorWithOptionalArgs is null)
		{
			return InvalidType(type, $"{type} needs to have a constructor with 0 args or only optional args.");
		}

		// get all optional default values
		var args = ctorWithOptionalArgs.GetParameters().Select(p => ToType(p.GetDefaultValue(), p.ParameterType));

		// create the ctor expression
		return New(ctorWithOptionalArgs, args);
	}

	/// <summary>
	/// Creates the collection.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="collectionType">The collection type.</param>
	/// <param name="genericArguments">The generic arguments.</param>
	/// <returns>An Expression.</returns>
	private static Expression CreateCollection(Type type, Type collectionType, Type[] genericArguments = null)
	{
		return ToType(New(collectionType.MakeGenericType(genericArguments ?? type.GenericTypeArguments)), type);
	}

	/// <summary>
	/// Creates the interface expression.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>An Expression.</returns>
	private static Expression CreateInterfaceExpression(Type type)
	{
		if (type.IsGenericType(Metadata.IDictionary2))
		{
			return CreateCollection(type, Metadata.Dictionary2);
		}
		else if (type.IsGenericType(Metadata.IReadOnlyDictionary2))
		{
			return CreateReadOnlyDictionary(type.GenericTypeArguments);
		}
		else if (type.IsGenericType(Metadata.ISet1))
		{
			return CreateCollection(type, Metadata.HashSet1);
		}
		else if (type.IsCollection())
		{
			return CreateCollection(type, Metadata.List1, GetIEnumerableArguments(type));
		}
		else
		{
			return InvalidType(type, $"Cannot create an instance of interface type {type}.");
		}
	}

	/// <summary>
	/// Creates the read only dictionary.
	/// </summary>
	/// <param name="typeArguments">The type arguments.</param>
	/// <returns>An Expression.</returns>
	private static Expression CreateReadOnlyDictionary(Type[] typeArguments)
	{
		var ctor = Metadata.ReadOnlyDictionary2.MakeGenericType(typeArguments).GetConstructors()[0];

		return New(ctor, New(Metadata.Dictionary2.MakeGenericType(typeArguments)));
	}

	/// <summary>
	/// Generates the constructor.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns><![CDATA[Func<object>]]></returns>
	private static Func<object> GenerateConstructor(Type type)
	{
		return Lambda<Func<object>>(GenerateConstructorExpression(type).ToObject()).CompileFast();
	}

	/// <summary>
	/// Gets the i enumerable arguments.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>An array of Types</returns>
	private static Type[] GetIEnumerableArguments(Type type)
	{
		return type.GetIEnumerableType()?.GenericTypeArguments ?? new[] { Metadata<object>.Type };
	}

	/// <summary>
	/// Invalids the type.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="message">The message.</param>
	/// <returns>An Expression.</returns>
	private static Expression InvalidType(Type type, string message)
	{
		return Throw(Constant(new ArgumentException(message, "type")), type);
	}
}