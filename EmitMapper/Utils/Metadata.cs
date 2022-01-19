using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

namespace EmitMapper.Utils;

/// <summary>
///   ÀàÐÍ×å
///   <para>2010/12/21</para>
///   <para>THINKPADT61</para>
///   <para>tangjingbo</para>
/// </summary>
public static class Metadata
{
  public static readonly Type Void = typeof(void);
  public static readonly Type List1 = typeof(List<>);
  public static readonly Type IList1 = typeof(IList<>);
  public static readonly Type Convert = typeof(Convert);
  public static readonly Type Math = typeof(Math);
  public static readonly Type ReadOnlyDictionary2 = typeof(ReadOnlyDictionary<,>);
  public static readonly Type Dictionary2 = typeof(Dictionary<,>);
  public static readonly Type IReadOnlyDictionary2 = typeof(IReadOnlyDictionary<,>);
  public static readonly Type ISet1 = typeof(ISet<>);
  public static readonly Type IDictionary2 = typeof(IDictionary<,>);
  public static readonly Type IEnumerable1 = typeof(IEnumerable<>);
  public static readonly Type Nullable1 = typeof(Nullable<>);
  public static readonly Type HashSet1 = typeof(HashSet<>);
  public static readonly Type ICollection1 = typeof(ICollection<>);

  public static readonly Type Func1 = typeof(Func<>);
  public static readonly Type Func2 = typeof(Func<,>);
  public static readonly Type Func3 = typeof(Func<,,>);
  public static readonly Type Func4 = typeof(Func<,,,>);
  public static readonly Type Func5 = typeof(Func<,,,,>);
  public static readonly Type Func6 = typeof(Func<,,,,,>);
  public static readonly Type Func7 = typeof(Func<,,,,,,>);
  public static readonly Type Func8 = typeof(Func<,,,,,,,>);

  public static readonly Type Action1 = typeof(Action<>);
  public static readonly Type Action2 = typeof(Action<,>);
  public static readonly Type Action3 = typeof(Action<,,>);
  public static readonly Type Action4 = typeof(Action<,,,>);
  public static readonly Type Action5 = typeof(Action<,,,,>);
  public static readonly Type Action6 = typeof(Action<,,,,,>);
  public static readonly Type Action7 = typeof(Action<,,,,,,>);

  public static Type UnderlineType(Type t)
  {
    return null;
  }
}

public class Metadata<T>
{
  public static readonly Type Type = typeof(T);
  public static string Name => Type.Name;

  public static FieldInfo Field<TX>(Expression<Func<T, TX>> expression)
  {
    return (expression.Body as MemberExpression)?.Member as FieldInfo;
  }

  public static PropertyInfo Property<TX>(Expression<Func<T, TX>> expression)
  {
    return (expression.Body as MemberExpression)?.Member as PropertyInfo;
  }

  public static MethodInfo Method(Expression<Action<T>> expression)
  {
    return (expression.Body as MethodCallExpression)?.Method;
  }

  public static MethodInfo Method<TX>(Expression<Func<T, TX>> expression)
  {
    return (expression.Body as MethodCallExpression)?.Method;
  }

  public static Type[] GetInterfacesCache()
  {
    return Type.GetInterfacesCache();
  }
}