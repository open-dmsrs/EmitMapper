namespace EmitMapper;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using EmitMapper.Utils;

public static class TypeExtensions
{
  private static readonly LazyConcurrentDictionary<Type, string> cachedMethod = new(Environment.ProcessorCount, 1024);

  public static IEnumerable<T> AsEnumerable<T>(this T any)
  {
    yield return any;
  }

  public static IEnumerable<T> AsEnumerable<T>(this T p0, T p1)
  {
    yield return p0;
    yield return p1;
  }

  public static IEnumerable<T> AsEnumerable<T>(this T p0, T p1, T p2)
  {
    yield return p0;
    yield return p1;
    yield return p2;
  }

  public static IEnumerable<T> AsEnumerable<T>(this T p0, T p1, T p2, T p3)
  {
    yield return p0;
    yield return p1;
    yield return p2;
    yield return p3;
  }

  public static IEnumerable<T> Concat<T>(this IEnumerable<T> any, T p0)
  {
    return any.Concat(p0.AsEnumerable());
  }

  public static IEnumerable<T> Concat<T>(this IEnumerable<T> any, T p0, T p1)
  {
    return any.Concat(p0.AsEnumerable(p1));
  }

  public static IEnumerable<T> Concat<T>(this IEnumerable<T> any, T p0, T p1, T p2)
  {
    return any.Concat(p0.AsEnumerable(p1, p2));
  }

  public static IEnumerable<T> Concat<T>(this IEnumerable<T> any, T p0, T p1, T p2, T p3)
  {
    return any.Concat(p0.AsEnumerable(p1, p2, p3));
  }

  public static ConstructorInfo GetCachedConstructor(this Type type, Type[] types)
  {
    return type.GetTypeInfo().GetConstructor(types);
  }

  public static ConstructorInfo GetCachedConstructor(this Type type, int a, Type[] types)
  {
    return type.GetTypeInfo().GetConstructor(types);
  }

  public static ConstructorInfo GetCachedConstructor(this Type type, BindingFlags b, Type[] types, object c)
  {
    // todo: need to complete
    // throw new NotImplementedException();
    return type.GetTypeInfo().GetConstructor(types); // .FirstOrDefault(x =true);
  }

  public static FieldInfo GetCachedField(this Type type, string name)
  {
    return type.GetTypeInfo().GetField(name);
  }

  public static FieldInfo GetCachedField(this Type type, string name, BindingFlags bfs)
  {
    return type.GetTypeInfo().GetField(name, bfs);
  }

  public static FieldInfo[] GetCachedFields(this Type type)
  {
    return type.GetTypeInfo().GetFields();
  }

  public static Type[] GetCachedGenericArguments(this Type type)
  {
    return type.GetTypeInfo().GetGenericArguments();
  }

  public static MemberInfo[] GetCachedMembers(this Type type)
  {
    return type.GetTypeInfo().GetMembers();
  }

  public static MemberInfo[] GetCachedMembers(this Type type, BindingFlags bfs)
  {
    return type.GetTypeInfo().GetMembers(bfs);
  }

  public static MethodInfo GetCachedMethod(this Type type, string methodName)
  {
    return type.GetTypeInfo().GetMethod(methodName);
  }

  public static MethodInfo GetCachedMethod(this Type type, string methodName, BindingFlags flags)
  {
    return type.GetTypeInfo().GetMethod(methodName, flags);
  }

  public static MethodInfo GetCachedMethod(this Type type, string methodName, Type[] types)
  {
    return type.GetTypeInfo().GetMethod(methodName, types);
  }

  public static MethodInfo[] GetCachedMethods(this Type type, BindingFlags flags)
  {
    return type.GetTypeInfo().GetMethods(flags);
  }

  public static PropertyInfo[] GetCachedProperties(this Type type)
  {
    return type.GetTypeInfo().GetProperties();
  }

  public static PropertyInfo GetCachedProperty(this Type type, string propertyName)
  {
    return type.GetTypeInfo().GetProperty(propertyName);
  }

  public static Type[] GetCahcedInterfaces(this Type type)
  {
    return type.GetTypeInfo().GetInterfacesCache();
  }

  public static MemberInfo[] GetMember(this Type type, string name)
  {
    return type.GetTypeInfo().GetMember(name);
  }
}