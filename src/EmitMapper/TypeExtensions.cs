namespace EmitMapper;

/// <summary>
///   The type extensions.
/// </summary>
public static class TypeExtensions
{
	private static readonly LazyConcurrentDictionary<Type, string> cachedMethod = new(Environment.ProcessorCount, 1024);

	/// <summary>
	///   As the enumerable.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="any">The any.</param>
	/// <returns><![CDATA[IEnumerable<T>]]></returns>
	public static IEnumerable<T> AsEnumerable<T>(this T any)
	{
		yield return any;
	}

	/// <summary>
	///   As the enumerable.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="p0">The p0.</param>
	/// <param name="p1">The p1.</param>
	/// <returns><![CDATA[IEnumerable<T>]]></returns>
	public static IEnumerable<T> AsEnumerable<T>(this T p0, T p1)
	{
		yield return p0;
		yield return p1;
	}

	/// <summary>
	///   As the enumerable.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="p0">The p0.</param>
	/// <param name="p1">The p1.</param>
	/// <param name="p2">The p2.</param>
	/// <returns><![CDATA[IEnumerable<T>]]></returns>
	public static IEnumerable<T> AsEnumerable<T>(this T p0, T p1, T p2)
	{
		yield return p0;
		yield return p1;
		yield return p2;
	}

	/// <summary>
	///   As the enumerable.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="p0">The p0.</param>
	/// <param name="p1">The p1.</param>
	/// <param name="p2">The p2.</param>
	/// <param name="p3">The p3.</param>
	/// <returns><![CDATA[IEnumerable<T>]]></returns>
	public static IEnumerable<T> AsEnumerable<T>(this T p0, T p1, T p2, T p3)
	{
		yield return p0;
		yield return p1;
		yield return p2;
		yield return p3;
	}

	/// <summary>
	///   Concats the list of ts.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="any">The any.</param>
	/// <param name="p0">The p0.</param>
	/// <returns><![CDATA[IEnumerable<T>]]></returns>
	public static IEnumerable<T> Concat<T>(this IEnumerable<T> any, T p0)
	{
		return any.Concat(p0.AsEnumerable());
	}

	/// <summary>
	///   Concats the list of ts.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="any">The any.</param>
	/// <param name="p0">The p0.</param>
	/// <param name="p1">The p1.</param>
	/// <returns><![CDATA[IEnumerable<T>]]></returns>
	public static IEnumerable<T> Concat<T>(this IEnumerable<T> any, T p0, T p1)
	{
		return any.Concat(p0.AsEnumerable(p1));
	}

	/// <summary>
	///   Concats the list of ts.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="any">The any.</param>
	/// <param name="p0">The p0.</param>
	/// <param name="p1">The p1.</param>
	/// <param name="p2">The p2.</param>
	/// <returns><![CDATA[IEnumerable<T>]]></returns>
	public static IEnumerable<T> Concat<T>(this IEnumerable<T> any, T p0, T p1, T p2)
	{
		return any.Concat(p0.AsEnumerable(p1, p2));
	}

	/// <summary>
	///   Concats the list of ts.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="any">The any.</param>
	/// <param name="p0">The p0.</param>
	/// <param name="p1">The p1.</param>
	/// <param name="p2">The p2.</param>
	/// <param name="p3">The p3.</param>
	/// <returns><![CDATA[IEnumerable<T>]]></returns>
	public static IEnumerable<T> Concat<T>(this IEnumerable<T> any, T p0, T p1, T p2, T p3)
	{
		return any.Concat(p0.AsEnumerable(p1, p2, p3));
	}

	/// <summary>
	///   Gets the cached constructor.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="types">The types.</param>
	/// <returns>A ConstructorInfo.</returns>
	public static ConstructorInfo GetCachedConstructor(this Type type, Type[] types)
	{
		return type.GetTypeInfo().GetConstructor(types);
	}

	/// <summary>
	///   Gets the cached constructor.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="a">The a.</param>
	/// <param name="types">The types.</param>
	/// <returns>A ConstructorInfo.</returns>
	public static ConstructorInfo GetCachedConstructor(this Type type, int a, Type[] types)
	{
		return type.GetTypeInfo().GetConstructor(types);
	}

	/// <summary>
	///   Gets the cached constructor.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="b">The b.</param>
	/// <param name="types">The types.</param>
	/// <param name="c">The c.</param>
	/// <exception cref="NotImplementedException"></exception>
	/// <returns>A ConstructorInfo.</returns>
	public static ConstructorInfo GetCachedConstructor(this Type type, BindingFlags b, Type[] types, object c)
	{
		// todo: need to complete
		// throw new NotImplementedException();
		return type.GetTypeInfo().GetConstructor(types); // .FirstOrDefault(x =true);
	}

	/// <summary>
	///   Gets the cached field.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="name">The name.</param>
	/// <returns>A FieldInfo.</returns>
	public static FieldInfo GetCachedField(this Type type, string name)
	{
		return type.GetTypeInfo().GetField(name);
	}

	/// <summary>
	///   Gets the cached field.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="name">The name.</param>
	/// <param name="bfs">The bfs.</param>
	/// <returns>A FieldInfo.</returns>
	public static FieldInfo GetCachedField(this Type type, string name, BindingFlags bfs)
	{
		return type.GetTypeInfo().GetField(name, bfs);
	}

	/// <summary>
	///   Gets the cached fields.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>An array of FieldInfos</returns>
	public static FieldInfo[] GetCachedFields(this Type type)
	{
		return type.GetTypeInfo().GetFields();
	}

	/// <summary>
	///   Gets the cached generic arguments.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>An array of Types</returns>
	public static Type[] GetCachedGenericArguments(this Type type)
	{
		return type.GetTypeInfo().GetGenericArguments();
	}

	/// <summary>
	///   Gets the cached members.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>An array of MemberInfos</returns>
	public static MemberInfo[] GetCachedMembers(this Type type)
	{
		return type.GetTypeInfo().GetMembers();
	}

	/// <summary>
	///   Gets the cached members.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="bfs">The bfs.</param>
	/// <returns>An array of MemberInfos</returns>
	public static MemberInfo[] GetCachedMembers(this Type type, BindingFlags bfs)
	{
		return type.GetTypeInfo().GetMembers(bfs);
	}

	/// <summary>
	///   Gets the cached method.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="methodName">The method name.</param>
	/// <returns>A MethodInfo.</returns>
	public static MethodInfo GetCachedMethod(this Type type, string methodName)
	{
		return type.GetTypeInfo().GetMethod(methodName);
	}

	/// <summary>
	///   Gets the cached method.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="methodName">The method name.</param>
	/// <param name="flags">The flags.</param>
	/// <returns>A MethodInfo.</returns>
	public static MethodInfo GetCachedMethod(this Type type, string methodName, BindingFlags flags)
	{
		return type.GetTypeInfo().GetMethod(methodName, flags);
	}

	/// <summary>
	///   Gets the cached method.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="methodName">The method name.</param>
	/// <param name="types">The types.</param>
	/// <returns>A MethodInfo.</returns>
	public static MethodInfo GetCachedMethod(this Type type, string methodName, Type[] types)
	{
		return type.GetTypeInfo().GetMethod(methodName, types);
	}

	/// <summary>
	///   Gets the cached methods.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="flags">The flags.</param>
	/// <returns>An array of MethodInfos</returns>
	public static MethodInfo[] GetCachedMethods(this Type type, BindingFlags flags)
	{
		return type.GetTypeInfo().GetMethods(flags);
	}

	/// <summary>
	///   Gets the cached properties.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>An array of PropertyInfos</returns>
	public static PropertyInfo[] GetCachedProperties(this Type type)
	{
		return type.GetTypeInfo().GetProperties();
	}

	/// <summary>
	///   Gets the cached property.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="propertyName">The property name.</param>
	/// <returns>A PropertyInfo.</returns>
	public static PropertyInfo GetCachedProperty(this Type type, string propertyName)
	{
		return type.GetTypeInfo().GetProperty(propertyName);
	}

	/// <summary>
	///   Gets the cahced interfaces.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>An array of Types</returns>
	public static Type[] GetCahcedInterfaces(this Type type)
	{
		return type.GetTypeInfo().GetInterfacesCache();
	}

	/// <summary>
	///   Gets the member.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="name">The name.</param>
	/// <returns>An array of MemberInfos</returns>
	public static MemberInfo[] GetMember(this Type type, string name)
	{
		return type.GetTypeInfo().GetMember(name);
	}
}