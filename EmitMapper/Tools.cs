using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EmitMapper.Utils;

namespace EmitMapper;

internal static class Tools
{
  internal static MethodInfo DelegateTargetGetterMethod =
    Metadata<Delegate>.Type.GetProperty(nameof(Delegate.Target)).GetMethod;

  public static T[] AsArray<T>(this IEnumerable<T> xs)
  {
    if (xs is T[] array)
      return array;

    return xs == null ? null : xs.ToArray();
  }

  public static T[] Empty<T>()
  {
    return EmptyArray<T>.Value;
  }

  public static string GetArithmeticBinaryOperatorMethodName(this ExpressionType nodeType)
  {
    return nodeType switch
    {
      ExpressionType.Add => "op_Addition",
      ExpressionType.AddChecked => "op_Addition",
      ExpressionType.Subtract => "op_Subtraction",
      ExpressionType.SubtractChecked => "op_Subtraction",
      ExpressionType.Multiply => "op_Multiply",
      ExpressionType.MultiplyChecked => "op_Multiply",
      ExpressionType.Divide => "op_Division",
      ExpressionType.Modulo => "op_Modulus",
      _ => null
    };
  }

  public static T GetFirst<T>(this IEnumerable<T> source)
  {
    // This is pretty much Linq.FirstOrDefault except it does not need to check
    // if source is IPartition<T> (but should it?)

    if (source is IList<T> list)
      return list.Count == 0 ? default : list[0];

    using (var items = source.GetEnumerator())
    {
      return items.MoveNext() ? items.Current : default;
    }
  }

  public static T GetFirst<T>(this T[] source)
  {
    return source.Length == 0 ? default : source[0];
  }

  public static Type GetFuncOrActionType(Type returnType)
  {
    return returnType == Metadata.Void ? Metadata<Action>.Type : Metadata.Func1.MakeGenericType(returnType);
  }

  public static Type GetFuncOrActionType(Type p, Type returnType)
  {
    return returnType == Metadata.Void
      ? Metadata.Action1.MakeGenericType(p)
      : Metadata.Func2.MakeGenericType(p, returnType);
  }

  public static Type GetFuncOrActionType(Type p0, Type p1, Type returnType)
  {
    return returnType == Metadata.Void
      ? Metadata.Action2.MakeGenericType(p0, p1)
      : Metadata.Func3.MakeGenericType(p0, p1, returnType);
  }

  public static Type GetFuncOrActionType(Type p0, Type p1, Type p2, Type returnType)
  {
    return returnType == Metadata.Void
      ? Metadata.Action3.MakeGenericType(p0, p1, p2)
      : Metadata.Func4.MakeGenericType(p0, p1, p2, returnType);
  }

  public static Type GetFuncOrActionType(Type p0, Type p1, Type p2, Type p3, Type returnType)
  {
    return returnType == Metadata.Void
      ? Metadata.Action4.MakeGenericType(p0, p1, p2, p3)
      : Metadata.Func5.MakeGenericType(p0, p1, p2, p3, returnType);
  }

  public static Type GetFuncOrActionType(Type p0, Type p1, Type p2, Type p3, Type p4, Type returnType)
  {
    return returnType == Metadata.Void
      ? Metadata.Action5.MakeGenericType(p0, p1, p2, p3, p4)
      : Metadata.Func6.MakeGenericType(p0, p1, p2, p3, p4, returnType);
  }

  public static Type GetFuncOrActionType(Type p0, Type p1, Type p2, Type p3, Type p4, Type p5, Type returnType)
  {
    return returnType == Metadata.Void
      ? Metadata.Action6.MakeGenericType(p0, p1, p2, p3, p4, p5)
      : Metadata.Func7.MakeGenericType(p0, p1, p2, p3, p4, p5, returnType);
  }

  public static Type GetFuncOrActionType(Type[] paramTypes, Type returnType)
  {
    if (returnType == Metadata.Void)
      switch (paramTypes.Length)
      {
        case 0: return Metadata<Action>.Type;
        case 1: return Metadata.Action1.MakeGenericType(paramTypes);
        case 2: return Metadata.Action2.MakeGenericType(paramTypes);
        case 3: return Metadata.Action3.MakeGenericType(paramTypes);
        case 4: return Metadata.Action4.MakeGenericType(paramTypes);
        case 5: return Metadata.Action5.MakeGenericType(paramTypes);
        case 6: return Metadata.Action6.MakeGenericType(paramTypes);
        case 7: return Metadata.Action7.MakeGenericType(paramTypes);
        default:
          throw new NotSupportedException(
            $"Action with so many ({paramTypes.Length}) parameters is not supported!");
      }

    switch (paramTypes.Length)
    {
      case 0: return Metadata.Func1.MakeGenericType(returnType);
      case 1: return Metadata.Func2.MakeGenericType(paramTypes[0], returnType);
      case 2: return Metadata.Func3.MakeGenericType(paramTypes[0], paramTypes[1], returnType);
      case 3: return Metadata.Func4.MakeGenericType(paramTypes[0], paramTypes[1], paramTypes[2], returnType);
      case 4:
        return Metadata.Func5.MakeGenericType(paramTypes[0], paramTypes[1], paramTypes[2], paramTypes[3], returnType);
      case 5:
        return Metadata.Func6.MakeGenericType(
          paramTypes[0],
          paramTypes[1],
          paramTypes[2],
          paramTypes[3],
          paramTypes[4],
          returnType);
      case 6:
        return Metadata.Func7.MakeGenericType(
          paramTypes[0],
          paramTypes[1],
          paramTypes[2],
          paramTypes[3],
          paramTypes[4],
          paramTypes[5],
          returnType);
      case 7:
        return Metadata.Func8.MakeGenericType(
          paramTypes[0],
          paramTypes[1],
          paramTypes[2],
          paramTypes[3],
          paramTypes[4],
          paramTypes[5],
          paramTypes[6],
          returnType);
      default:
        throw new NotSupportedException(
          $"Func with so many ({paramTypes.Length}) parameters is not supported!");
    }
  }

  public static Type[] GetParamTypes(IReadOnlyList<ParameterExpression> paramExprs)
  {
    if (paramExprs == null)
      return Empty<Type>();

    var count = paramExprs.Count;

    if (count == 0)
      return Empty<Type>();

    if (count == 1)
      return new[] { paramExprs[0].IsByRef ? paramExprs[0].Type.MakeByRefType() : paramExprs[0].Type };

    var paramTypes = new Type[count];

    for (var i = 0; i < paramTypes.Length; i++)
    {
      var parameterExpr = paramExprs[i];
      paramTypes[i] = parameterExpr.IsByRef ? parameterExpr.Type.MakeByRefType() : parameterExpr.Type;
    }

    return paramTypes;
  }

  internal static MethodInfo FindConvertOperator(this Type type, Type sourceType, Type targetType)
  {
    var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

    for (var i = 0; i < methods.Length; i++)
    {
      var m = methods[i];

      if (m.IsSpecialName && m.ReturnType == targetType)
      {
        var n = m.Name;

        // n == "op_Implicit" || n == "op_Explicit"
        if (n.Length == 11 &&
            n[2] == '_' && n[5] == 'p' && n[6] == 'l' && n[7] == 'i' && n[8] == 'c' && n[9] == 'i' && n[10] == 't' &&
            m.GetParameters()[0].ParameterType == sourceType)
          return m;
      }
    }

    return null;
  }

  internal static MethodInfo FindDelegateInvokeMethod(this Type type)
  {
    return type.GetMethodCache("Invoke");
  }

  internal static MethodInfo FindMethod(this Type type, string methodName)
  {
    var methods = type.GetMethods();

    for (var i = 0; i < methods.Length; i++)
      if (methods[i].Name == methodName)
        return methods[i];

    return type.BaseType?.FindMethod(methodName);
  }

  internal static MethodInfo FindNullableGetValueOrDefaultMethod(this Type type)
  {
    var methods = type.GetMethods();

    for (var i = 0; i < methods.Length; i++)
    {
      var m = methods[i];

      if (m.GetParameters().Length == 0 && m.Name == "GetValueOrDefault")
        return m;
    }

    return null;
  }

  internal static MethodInfo FindNullableHasValueGetterMethod(this Type type)
  {
    return type.GetProperty("HasValue").GetMethod;
  }

  internal static ConstructorInfo FindSingleParamConstructor(this Type type, Type paramType)
  {
    var ctors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    for (var i = 0; i < ctors.Length; i++)
    {
      var ctor = ctors[i];
      var parameters = ctor.GetParameters();

      if (parameters.Length == 1 && parameters[0].ParameterType == paramType)
        return ctor;
    }

    return null;
  }

  internal static MethodInfo FindValueGetterMethod(this Type type)
  {
    return type.GetProperty("Value").GetMethod;
  }

  internal static bool IsNullable(this Type type)
  {
    return type.IsGenericType && type.GetGenericTypeDefinitionCache() == Metadata.Nullable1;
  }

  internal static bool IsUnsigned(this Type type)
  {
    return type == Metadata<byte>.Type ||
           type == Metadata<ushort>.Type ||
           type == Metadata<uint>.Type ||
           type == Metadata<ulong>.Type;
  }

  private static class EmptyArray<T>
  {
    public static readonly T[] Value = new T[0];
  }
}