namespace EmitMapper.Utils;

/// <summary>
/// <para>2010/12/21</para>
/// <para>THINKPADT61</para>
/// <para>tangjingbo</para>
/// </summary>
public static class Metadata
{
	/// <summary>
	/// The action.
	/// </summary>
	public static readonly Type Action = Metadata<Action>.Type;

	/// <summary>
	/// The action1.
	/// </summary>
	public static readonly Type Action1 = typeof(Action<>);

	/// <summary>
	/// The action2.
	/// </summary>
	public static readonly Type Action2 = typeof(Action<,>);

	/// <summary>
	/// The action3.
	/// </summary>
	public static readonly Type Action3 = typeof(Action<,,>);

	/// <summary>
	/// The action4.
	/// </summary>
	public static readonly Type Action4 = typeof(Action<,,,>);

	/// <summary>
	/// The action5.
	/// </summary>
	public static readonly Type Action5 = typeof(Action<,,,,>);

	/// <summary>
	/// The action6.
	/// </summary>
	public static readonly Type Action6 = typeof(Action<,,,,,>);

	/// <summary>
	/// The action7.
	/// </summary>
	public static readonly Type Action7 = typeof(Action<,,,,,,>);

	/// <summary>
	/// The convert.
	/// </summary>
	public static readonly Type Convert = typeof(Convert);

	/// <summary>
	/// The dictionary2.
	/// </summary>
	public static readonly Type Dictionary2 = typeof(Dictionary<,>);

	/// <summary>
	/// The func1.
	/// </summary>
	public static readonly Type Func1 = typeof(Func<>);

	/// <summary>
	/// The func2.
	/// </summary>
	public static readonly Type Func2 = typeof(Func<,>);

	/// <summary>
	/// The func3.
	/// </summary>
	public static readonly Type Func3 = typeof(Func<,,>);

	/// <summary>
	/// The func4.
	/// </summary>
	public static readonly Type Func4 = typeof(Func<,,,>);

	/// <summary>
	/// The func5.
	/// </summary>
	public static readonly Type Func5 = typeof(Func<,,,,>);

	/// <summary>
	/// The func6.
	/// </summary>
	public static readonly Type Func6 = typeof(Func<,,,,,>);

	/// <summary>
	/// The func7.
	/// </summary>
	public static readonly Type Func7 = typeof(Func<,,,,,,>);

	/// <summary>
	/// The func8.
	/// </summary>
	public static readonly Type Func8 = typeof(Func<,,,,,,,>);

	/// <summary>
	/// The hash set1.
	/// </summary>
	public static readonly Type HashSet1 = typeof(HashSet<>);

	/// <summary>
	/// The collection1.
	/// </summary>
	public static readonly Type Collection1 = typeof(ICollection<>);

	/// <summary>
	/// The i dictionary2.
	/// </summary>
	public static readonly Type IDictionary2 = typeof(IDictionary<,>);

	/// <summary>
	/// The enumerable1.
	/// </summary>
	public static readonly Type Enumerable1 = typeof(IEnumerable<>);

	/// <summary>
	/// The i list1.
	/// </summary>
	public static readonly Type IList1 = typeof(IList<>);

	/// <summary>
	/// The i read only dictionary2.
	/// </summary>
	public static readonly Type IReadOnlyDictionary2 = typeof(IReadOnlyDictionary<,>);

	/// <summary>
	/// The set1.
	/// </summary>
	public static readonly Type Set1 = typeof(ISet<>);

	/// <summary>
	/// List&lt;&gt;
	/// </summary>
	public static readonly Type List1 = typeof(List<>);

	/// <summary>
	/// The math.
	/// </summary>
	public static readonly Type Math = typeof(Math);

	/// <summary>
	/// The nullable1.
	/// </summary>
	public static readonly Type Nullable1 = typeof(Nullable<>);

	/// <summary>
	/// The read only dictionary2.
	/// </summary>
	public static readonly Type ReadOnlyDictionary2 = typeof(ReadOnlyDictionary<,>);

	/// <summary>
	/// Void
	/// </summary>
	public static readonly Type Void = typeof(void);
}

/// <summary>
/// The action metadata.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ActionMetadata<T> : Metadata<Action<T>>;

/// <summary>
/// The action metadata.
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
public class ActionMetadata<T1, T2> : Metadata<Action<T1, T2>>;

/// <summary>
/// The action metadata.
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
public class ActionMetadata<T1, T2, T3> : Metadata<Action<T1, T2, T3>>;

/// <summary>
/// The action metadata.
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
public class ActionMetadata<T1, T2, T3, T4> : Metadata<Action<T1, T2, T3, T4>>;

/// <summary>
/// The action metadata.
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
/// <typeparam name="T5"></typeparam>
public class ActionMetadata<T1, T2, T3, T4, T5> : Metadata<Action<T1, T2, T3, T4, T5>>;

/// <summary>
/// The action metadata.
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
/// <typeparam name="T5"></typeparam>
/// <typeparam name="T6"></typeparam>
public class ActionMetadata<T1, T2, T3, T4, T5, T6> : Metadata<Action<T1, T2, T3, T4, T5, T6>>;

/// <summary>
/// The action metadata.
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
/// <typeparam name="T5"></typeparam>
/// <typeparam name="T6"></typeparam>
/// <typeparam name="T7"></typeparam>
public class ActionMetadata<T1, T2, T3, T4, T5, T6, T7> : Metadata<Action<T1, T2, T3, T4, T5, T6, T7>>;

/// <summary>
/// The func metadata.
/// </summary>
/// <typeparam name="T"></typeparam>
public class FuncMetadata<T> : Metadata<Func<T>>;

/// <summary>
/// The func metadata.
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="TR"></typeparam>
public class FuncMetadata<T1, TR> : Metadata<Func<T1, TR>>;

/// <summary>
/// The func metadata.
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="TR"></typeparam>
public class FuncMetadata<T1, T2, TR> : Metadata<Func<T1, T2, TR>>;

/// <summary>
/// The func metadata.
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="TR"></typeparam>
public class FuncMetadata<T1, T2, T3, TR> : Metadata<Func<T1, T2, T3, TR>>;

/// <summary>
/// The func metadata.
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
/// <typeparam name="TR"></typeparam>
public class FuncMetadata<T1, T2, T3, T4, TR> : Metadata<Func<T1, T2, T3, T4, TR>>;

/// <summary>
/// The func metadata.
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
/// <typeparam name="T5"></typeparam>
/// <typeparam name="TR"></typeparam>
public class FuncMetadata<T1, T2, T3, T4, T5, TR> : Metadata<Func<T1, T2, T3, T4, T5, TR>>;

/// <summary>
/// The metadata.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Metadata<T>
{
	/// <summary>
	/// The type.
	/// </summary>
	public static readonly Type Type = typeof(T);

	/// <summary>
	/// Initializes a new instance of the <see cref="Metadata{T}"/> class.
	/// </summary>
	protected Metadata()
	{
	}

	/// <summary>
	/// Gets the type name.
	/// </summary>
	public static string TypeName => Type.Name;

	/// <summary>
	/// Gets the interfaces cache.
	/// </summary>
	/// <returns>An array of Types</returns>
	public static Type[] GetInterfacesCache()
	{
		return Type.GetInterfacesCache();
	}
}

/// <summary>
/// The func metadata.
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
/// <typeparam name="T5"></typeparam>
/// <typeparam name="T6"></typeparam>
/// <typeparam name="TR"></typeparam>
public class FuncMetadata<T1, T2, T3, T4, T5, T6, TR> : Metadata<Func<T1, T2, T3, T4, T5, T6, TR>>;
