using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using EmitMapper.Utils;

namespace EmitMapper;

public static class CodePrinter
{
  private const string _nonPubInstMethods = "BindingFlags.NonPublic|BindingFlags.Instance";

  private const string _nonPubStatMethods = "BindingFlags.NonPublic|BindingFlags.Static";

  internal static readonly IObjectToCode DefaultConstantValueToCode = new ConstantValueToCode();

  private static readonly Type[] TypesImplementedByArray =
    Metadata<object[]>.GetInterfacesCache().Where(t => t.GetTypeInfo().IsGenericType)
      .Select(t => t.GetGenericTypeDefinitionCache()).ToArray();

  public interface IObjectToCode
  {
    string ToCode(object x, bool stripNamespace = false, Func<Type, string, string> printType = null);
  }

  public static StringBuilder AppendMethod(this StringBuilder sb, MethodInfo method,
    bool stripNamespace = false, Func<Type, string, string> printType = null)
  {
    if (method == null)
      return sb.Append("null");

    sb.AppendTypeof(method.DeclaringType, stripNamespace, printType);
    sb.Append(".GetMethods(");

    if (!method.IsPublic)
      sb.Append(method.IsStatic ? _nonPubStatMethods : _nonPubInstMethods);

    var mp = method.GetParameters();

    if (!method.IsGenericMethod)
    {
      sb.Append(").Single(x => !x.IsGenericMethod && x.Name == \"").Append(method.Name).Append("\" && ");

      return mp.Length == 0
        ? sb.Append("x.GetParameters().Length == 0)")
        : sb.Append("x.GetParameters().Select(y => y.ParameterType).SequenceEqual(new[] { ")
          .AppendTypeofList(mp.Select(x => x.ParameterType).ToArray(), stripNamespace, printType)
          .Append(" }))");
    }

    var tp = method.GetGenericArguments();
    sb.Append(").Where(x => x.IsGenericMethod && x.Name == \"").Append(method.Name).Append("\" && ");

    if (mp.Length == 0)
    {
      sb.Append("x.GetParameters().Length == 0 && x.GetGenericArguments().Length == ").Append(tp.Length);

      sb.Append(").Select(x => x.IsGenericMethodDefinition ? x.MakeGenericMethod(")
        .AppendTypeofList(tp, stripNamespace, printType);

      return sb.Append(") : x).Single()");
    }

    sb.Append("x.GetGenericArguments().Length == ").Append(tp.Length);

    sb.Append(").Select(x => x.IsGenericMethodDefinition ? x.MakeGenericMethod(")
      .AppendTypeofList(tp, stripNamespace, printType);

    sb.Append(") : x).Single(x => x.GetParameters().Select(y => y.ParameterType).SequenceEqual(new[] { ");
    sb.AppendTypeofList(mp.Select(x => x.ParameterType).ToArray(), stripNamespace, printType);

    return sb.Append(" }))");
  }

  public static StringBuilder AppendTypeof(this StringBuilder sb, Type type,
    bool stripNamespace = false, Func<Type, string, string> printType = null, bool printGenericTypeArgs = false)
  {
    if (type == null)
      return sb.Append("null");

    sb.Append("typeof(").Append(type.ToCode(stripNamespace, printType, printGenericTypeArgs)).Append(')');

    return type.IsByRef ? sb.Append(".MakeByRefType()") : sb;
  }

  public static StringBuilder AppendTypeofList(this StringBuilder sb, Type[] types,
    bool stripNamespace = false, Func<Type, string, string> printType = null, bool printGenericTypeArgs = false)
  {
    for (var i = 0; i < types.Length; i++)
      (i > 0 ? sb.Append(", ") : sb).AppendTypeof(types[i], stripNamespace, printType, printGenericTypeArgs);

    return sb;
  }

  /// <summary>Prints many code items as array initializer.</summary>
  public static string ToArrayInitializerCode(this IEnumerable items, Type itemType, IObjectToCode notRecognizedToCode,
    bool stripNamespace = false, Func<Type, string, string> printType = null)
  {
    return
      $"new {itemType.ToCode(stripNamespace, printType)}[]{{{items.ToCommaSeparatedCode(notRecognizedToCode, stripNamespace, printType)}}}";
  }

  /// <summary>Converts the <paramref name="type" /> into the proper C# representation.</summary>
  public static string ToCode(this Type type,
    bool stripNamespace = false, Func<Type, string, string> printType = null, bool printGenericTypeArgs = false)
  {
    if (type.IsGenericParameter)
      return !printGenericTypeArgs
        ? string.Empty
        : printType?.Invoke(type, type.Name) ?? type.Name;

    if (type.GetUnderlyingTypeCache() is Type nullableElementType && !type.IsGenericTypeDefinition)
    {
      var result = nullableElementType.ToCode(stripNamespace, printType, printGenericTypeArgs) + "?";

      return printType?.Invoke(type, result) ?? result;
    }

    Type arrayType = null;

    if (type.IsArray)
    {
      // store the original type for the later and process its element type further here
      arrayType = type;
      type = type.GetElementType();
    }

    // the default handling of the built-in types
    string buildInTypeString = null;

    if (type == Metadata.Void)
      buildInTypeString = "void";
    else if (type == Metadata<object>.Type)
      buildInTypeString = "object";
    else if (type == Metadata<bool>.Type)
      buildInTypeString = "bool";
    else if (type == Metadata<int>.Type)
      buildInTypeString = "int";
    else if (type == Metadata<short>.Type)
      buildInTypeString = "short";
    else if (type == Metadata<byte>.Type)
      buildInTypeString = "byte";
    else if (type == Metadata<double>.Type)
      buildInTypeString = "double";
    else if (type == Metadata<float>.Type)
      buildInTypeString = "float";
    else if (type == Metadata<char>.Type)
      buildInTypeString = "char";
    else if (type == Metadata<string>.Type)
      buildInTypeString = "string";

    if (buildInTypeString != null)
    {
      if (arrayType != null)
        buildInTypeString += "[]";

      return printType?.Invoke(arrayType ?? type, buildInTypeString) ?? buildInTypeString;
    }

    var parentCount = 0;

    for (var ti = type.GetTypeInfo(); ti.IsNested; ti = ti.DeclaringType.GetTypeInfo())
      ++parentCount;

    Type[] parentTypes = null;

    if (parentCount > 0)
    {
      parentTypes = new Type[parentCount];
      var pt = type.DeclaringType;

      for (var i = 0; i < parentTypes.Length; i++, pt = pt.DeclaringType)
        parentTypes[i] = pt;
    }

    var typeInfo = type.GetTypeInfo();
    Type[] typeArgs = null;
    var isTypeClosedGeneric = false;

    if (type.IsGenericType)
    {
      isTypeClosedGeneric = !typeInfo.IsGenericTypeDefinition;
      typeArgs = isTypeClosedGeneric ? typeInfo.GenericTypeArguments : typeInfo.GenericTypeParameters;
    }

    var typeArgsConsumedByParentsCount = 0;
    var s = new StringBuilder();

    if (!stripNamespace &&
        !string.IsNullOrEmpty(
          type.Namespace)) // for the auto-generated classes Namespace may be empty and in general it may be empty
      s.Append(type.Namespace).Append('.');

    if (parentTypes != null)
      for (var p = parentTypes.Length - 1; p >= 0; --p)
      {
        var parentType = parentTypes[p];

        if (!parentType.IsGenericType)
        {
          s.Append(parentType.Name).Append('.');
        }
        else
        {
          var parentTypeInfo = parentType.GetTypeInfo();
          Type[] parentTypeArgs = null;

          if (parentTypeInfo.IsGenericTypeDefinition)
          {
            parentTypeArgs = parentTypeInfo.GenericTypeParameters;

            // replace the open parent args with the closed child args,
            // and close the parent
            if (isTypeClosedGeneric)
              for (var t = 0; t < parentTypeArgs.Length; ++t)
                parentTypeArgs[t] = typeArgs[t];

            var parentTypeArgCount = parentTypeArgs.Length;

            if (typeArgsConsumedByParentsCount > 0)
            {
              var ownArgCount = parentTypeArgCount - typeArgsConsumedByParentsCount;

              if (ownArgCount == 0)
              {
                parentTypeArgs = null;
              }
              else
              {
                var ownArgs = new Type[ownArgCount];

                for (var a = 0; a < ownArgs.Length; ++a)
                  ownArgs[a] = parentTypeArgs[a + typeArgsConsumedByParentsCount];

                parentTypeArgs = ownArgs;
              }
            }

            typeArgsConsumedByParentsCount = parentTypeArgCount;
          }
          else
          {
            parentTypeArgs = parentTypeInfo.GenericTypeArguments;
          }

          var parentTickIndex = parentType.Name.IndexOf('`');
          s.Append(parentType.Name.Substring(0, parentTickIndex));

          // The owned parentTypeArgs maybe empty because all args are defined in the parent's parents
          if (parentTypeArgs?.Length > 0)
          {
            s.Append('<');

            for (var t = 0; t < parentTypeArgs.Length; ++t)
              (t == 0 ? s : s.Append(", "))
                .Append(parentTypeArgs[t].ToCode(stripNamespace, printType, printGenericTypeArgs));

            s.Append('>');
          }

          s.Append('.');
        }
      }

    var name = type.Name.TrimStart('<', '>').TrimEnd('&');

    if (typeArgs != null && typeArgsConsumedByParentsCount < typeArgs.Length)
    {
      var tickIndex = name.IndexOf('`');
      s.Append(name.Substring(0, tickIndex)).Append('<');

      for (var i = 0; i < typeArgs.Length - typeArgsConsumedByParentsCount; ++i)
        (i == 0 ? s : s.Append(", "))
          .Append(
            typeArgs[i + typeArgsConsumedByParentsCount]
              .ToCode(stripNamespace, printType, printGenericTypeArgs));

      s.Append('>');
    }
    else
    {
      s.Append(name);
    }

    if (arrayType != null)
      s.Append("[]");

    return printType?.Invoke(arrayType ?? type, s.ToString()) ?? s.ToString();
  }

  /// <summary>Prints valid C# Boolean</summary>
  public static string ToCode(this bool x)
  {
    return x ? "true" : "false";
  }

  /// <summary>Prints valid C# String escaping the things</summary>
  public static string ToCode(this string x)
  {
    return x == null ? "null" : $"\"{x.Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n")}\"";
  }

  /// <summary>
  ///   Prints a valid C# for known <paramref name="x" />,
  ///   otherwise uses passed <paramref name="notRecognizedToCode" /> or falls back to `ToString()`.
  /// </summary>
  public static string ToCode(this object x, IObjectToCode notRecognizedToCode,
    bool stripNamespace = false, Func<Type, string, string> printType = null)
  {
    if (x == null)
      return "null";

    if (x is bool b)
      return b.ToCode();

    if (x is int i)
      return i.ToString();

    if (x is double d)
      return d.ToString();

    if (x is string s)
      return s.ToCode();

    if (x is char c)
      return "'" + c + "'";

    if (x is Type t)
      return t.ToCode(stripNamespace, printType);

    if (x is Guid guid)
      return "Guid.Parse(" + guid.ToString().ToCode() + ")";

    if (x is DateTime date)
      return "DateTime.Parse(" + date.ToString().ToCode() + ")";

    if (x is TimeSpan time)
      return "TimeSpan.Parse(" + time.ToString().ToCode() + ")";

    var xType = x.GetType();
    var xTypeInfo = xType.GetTypeInfo();

    // check if item is implemented by array and then use the array initializer only for these types, 
    // otherwise we may produce the array initializer but it will be incompatible with e.g. `List<T>`
    if (xTypeInfo.IsArray ||
        xTypeInfo.IsGenericType && TypesImplementedByArray.Contains(xType.GetGenericTypeDefinitionCache()))
    {
      var elemType = xTypeInfo.IsArray
        ? xTypeInfo.GetElementType()
        : xTypeInfo.GetGenericTypeParametersOrArguments().GetFirst();

      if (elemType != null)
        return ((IEnumerable)x).ToArrayInitializerCode(elemType, notRecognizedToCode);
    }

    // unwrap the Nullable struct
    if (xTypeInfo.IsGenericType && xTypeInfo.GetGenericTypeDefinitionCache() == Metadata.Nullable1)
    {
      xType = xTypeInfo.GetElementType();
      xTypeInfo = xType.GetTypeInfo();
    }

    if (xTypeInfo.IsEnum)
      return x.GetType().ToEnumValueCode(x, stripNamespace, printType);

    if (xTypeInfo.IsPrimitive) // output the primitive casted to the type
      return "(" + x.GetType().ToCode(true) + ")" + x;

    return notRecognizedToCode?.ToCode(x, stripNamespace, printType) ?? x.ToString();
  }

  /// <summary>Prints many code items as the array initializer.</summary>
  public static string ToCommaSeparatedCode(this IEnumerable items, IObjectToCode notRecognizedToCode,
    bool stripNamespace = false, Func<Type, string, string> printType = null)
  {
    var s = new StringBuilder();
    var first = true;

    foreach (var item in items)
    {
      if (first)
        first = false;
      else
        s.Append(", ");

      s.Append(item.ToCode(notRecognizedToCode, stripNamespace, printType));
    }

    return s.ToString();
  }

  /// <summary>Prints valid C# Enum literal</summary>
  public static string ToEnumValueCode(this Type enumType, object x,
    bool stripNamespace = false, Func<Type, string, string> printType = null)
  {
    return $"{enumType.ToCode(stripNamespace, printType)}.{Enum.GetName(enumType, x)}";
  }

  internal static StringBuilder AppendEnum<TEnum>(this StringBuilder sb, TEnum value,
    bool stripNamespace = false, Func<Type, string, string> printType = null)
  {
    return sb.Append(Metadata<TEnum>.Type.ToCode(stripNamespace, printType)).Append('.')
      .Append(Enum.GetName(Metadata<TEnum>.Type, value));
  }

  internal static StringBuilder AppendField(this StringBuilder sb, FieldInfo field,
    bool stripNamespace = false, Func<Type, string, string> printType = null)
  {
    return sb.AppendTypeof(field.DeclaringType, stripNamespace, printType)
      .Append(".GetTypeInfo().GetDeclaredField(\"").Append(field.Name).Append("\")");
  }

  internal static StringBuilder AppendMember(this StringBuilder sb, MemberInfo member,
    bool stripNamespace = false, Func<Type, string, string> printType = null)
  {
    return member is FieldInfo f
      ? sb.AppendField(f, stripNamespace, printType)
      : sb.AppendProperty((PropertyInfo)member, stripNamespace, printType);
  }

  internal static StringBuilder AppendName<T>(this StringBuilder sb, string name, Type type, T identity)
  {
    return name != null
      ? sb.Append(name)
      : sb.Append(
          type.ToCode(true).Replace('.', '_').Replace('<', '_').Replace('>', '_').Replace(", ", "_").ToLowerInvariant())
        .Append("__").Append(identity.GetHashCode());
  }

  internal static StringBuilder AppendProperty(this StringBuilder sb, PropertyInfo property,
    bool stripNamespace = false, Func<Type, string, string> printType = null)
  {
    return sb.AppendTypeof(property.DeclaringType, stripNamespace, printType)
      .Append(".GetTypeInfo().GetDeclaredProperty(\"").Append(property.Name).Append("\")");
  }

  internal static StringBuilder NewLine(this StringBuilder sb, int lineIdent, int identSpaces)
  {
    return sb.AppendLine().Append(' ', Math.Max(lineIdent - identSpaces, 0));
  }

  internal static StringBuilder NewLineIdent(this StringBuilder sb, int lineIdent)
  {
    return sb.AppendLine().Append(' ', lineIdent);
  }

  internal static StringBuilder NewLineIdentArgumentExprs<T>(this StringBuilder sb, IReadOnlyList<T> exprs,
    List<ParameterExpression> paramsExprs, List<Expression> uniqueExprs, List<LabelTarget> lts,
    int lineIdent, bool stripNamespace, Func<Type, string, string> printType, int identSpaces,
    TryPrintConstant tryPrintConstant)
    where T : Expression
  {
    if (exprs.Count == 0)
      return sb.Append(" new ").Append(Metadata<T>.Type.ToCode(true)).Append("[0]");

    for (var i = 0; i < exprs.Count; i++)
      (i > 0 ? sb.Append(", ") : sb).NewLineIdentExpr(
        exprs[i],
        paramsExprs,
        uniqueExprs,
        lts,
        lineIdent,
        stripNamespace,
        printType,
        identSpaces,
        tryPrintConstant);

    return sb;
  }

  internal static StringBuilder NewLineIdentCs(this StringBuilder sb, Expression expr,
    int lineIdent, bool stripNamespace, Func<Type, string, string> printType, int identSpaces,
    TryPrintConstant tryPrintConstant)
  {
    sb.NewLineIdent(lineIdent);

    return expr?.ToCSharpString(
      sb,
      lineIdent + identSpaces,
      stripNamespace,
      printType,
      identSpaces,
      tryPrintConstant) ?? sb.Append("null");
  }

  internal static StringBuilder NewLineIdentExpr(this StringBuilder sb,
    Expression expr, List<ParameterExpression> paramsExprs, List<Expression> uniqueExprs, List<LabelTarget> lts,
    int lineIdent, bool stripNamespace, Func<Type, string, string> printType, int identSpaces,
    TryPrintConstant tryPrintConstant)
  {
    sb.NewLineIdent(lineIdent);

    return expr?.ToExpressionString(
      sb,
      paramsExprs,
      uniqueExprs,
      lts,
      lineIdent + identSpaces,
      stripNamespace,
      printType,
      identSpaces,
      tryPrintConstant) ?? sb.Append("null");
  }

  private static Type[] GetGenericTypeParametersOrArguments(this TypeInfo typeInfo)
  {
    return typeInfo.IsGenericTypeDefinition ? typeInfo.GenericTypeParameters : typeInfo.GenericTypeArguments;
  }

  private class ConstantValueToCode : IObjectToCode
  {
    public string ToCode(object x, bool stripNamespace = false, Func<Type, string, string> printType = null)
    {
      return "default(" + x.GetType().ToCode(stripNamespace, printType) + ") /* " + x +
             " !!! Please provide the non-default value */ ";
    }
  }
}