using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EmitMapper.Utils;

/// <summary>
///   
///   <para>2010/12/21</para>
///   <para>THINKPADT61</para>
///   <para>tangjingbo</para>
/// </summary>
public static class Metadata
{
  public static readonly Type Action = Metadata<Action>.Type;

  public static readonly Type Action1 = typeof(Action<>);

  public static readonly Type Action2 = typeof(Action<,>);

  public static readonly Type Action3 = typeof(Action<,,>);

  public static readonly Type Action4 = typeof(Action<,,,>);

  public static readonly Type Action5 = typeof(Action<,,,,>);

  public static readonly Type Action6 = typeof(Action<,,,,,>);

  public static readonly Type Action7 = typeof(Action<,,,,,,>);

  public static readonly Type Convert = typeof(Convert);

  public static readonly Type Dictionary2 = typeof(Dictionary<,>);

  public static readonly Type Func1 = typeof(Func<>);

  public static readonly Type Func2 = typeof(Func<,>);

  public static readonly Type Func3 = typeof(Func<,,>);

  public static readonly Type Func4 = typeof(Func<,,,>);

  public static readonly Type Func5 = typeof(Func<,,,,>);

  public static readonly Type Func6 = typeof(Func<,,,,,>);

  public static readonly Type Func7 = typeof(Func<,,,,,,>);

  public static readonly Type Func8 = typeof(Func<,,,,,,,>);

  public static readonly Type HashSet1 = typeof(HashSet<>);

  public static readonly Type ICollection1 = typeof(ICollection<>);

  public static readonly Type IDictionary2 = typeof(IDictionary<,>);

  public static readonly Type IEnumerable1 = typeof(IEnumerable<>);

  public static readonly Type IList1 = typeof(IList<>);

  public static readonly Type IReadOnlyDictionary2 = typeof(IReadOnlyDictionary<,>);

  public static readonly Type ISet1 = typeof(ISet<>);

  /// <summary>
  ///   List&lt;&gt;
  /// </summary>
  public static readonly Type List1 = typeof(List<>);

  public static readonly Type Math = typeof(Math);

  public static readonly Type Nullable1 = typeof(Nullable<>);

  public static readonly Type ReadOnlyDictionary2 = typeof(ReadOnlyDictionary<,>);

  public static readonly Type Void = typeof(void);

  /// <summary>
  ///   Underlines the type.
  /// </summary>
  /// <param name="t">The t.</param>
  /// <returns>A Type.</returns>
  [Obsolete]
  public static Type UnderlineType(Type t)
  {
    return null;
  }

  public static Type GetCachedType<T>(this object anyone)
  {
    return Metadata<T>.Type;
  }
}

/// <summary>
///   The metadata.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Metadata<T>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="Metadata"/> class.
  /// </summary>
  static Metadata()
  {
    Type = typeof(T);
  }
  public static readonly Type Type;

  /// <summary>
  ///   Gets the type name.
  /// </summary>
  public static string TypeName => Type.Name;

  /// <summary>
  ///   Gets the interfaces cache.
  /// </summary>
  /// <returns>An array of Types</returns>
  public static Type[] GetInterfacesCache()
  {
    return Type.GetInterfacesCache();
  }
}


public class FuncMetadata<T> : Metadata<Func<T>> { }
public class FuncMetadata<TA, TR> : Metadata<Func<TA, TR>> { }