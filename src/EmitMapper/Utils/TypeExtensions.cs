using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace EmitMapper.Utils;

/// <summary>
///   The type extensions.
/// </summary>
public static class TypeExtensions
{
  public const BindingFlags InstanceFlags =
    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

  public const BindingFlags StaticFlags =
    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

  /// <summary>
  ///   Bases the classes and interfaces.
  /// </summary>
  /// <param name="type">The type.</param>
  /// <returns><![CDATA[IEnumerable<Type>]]></returns>
  public static IEnumerable<Type> BaseClassesAndInterfaces(this Type type)
  {
    var currentType = type;

    while ((currentType = currentType.BaseType) != null) yield return currentType;
    foreach (var interfaceType in type.GetInterfacesCache()) yield return interfaceType;
  }

  /// <summary>
  ///   Checks the is derived from.
  /// </summary>
  /// <param name="derivedType">The derived type.</param>
  /// <param name="baseType">The base type.</param>
  /// <exception cref="ArgumentOutOfRangeException"></exception>
  public static void CheckIsDerivedFrom(this Type derivedType, Type baseType)
  {
    if (!baseType.IsAssignableFrom(derivedType) && !derivedType.IsGenericTypeDefinition
                                                && !baseType.IsGenericTypeDefinition)
      throw new ArgumentOutOfRangeException(nameof(derivedType), $"{derivedType} is not derived from {baseType}.");
  }

  /// <summary>
  ///   Generics the parameters count.
  /// </summary>
  /// <param name="type">The type.</param>
  /// <returns>An int.</returns>
  public static int GenericParametersCount(this Type type)
  {
    return type.GetTypeInfo().GenericTypeParameters.Length;
  }

  /// <summary>
  ///   Gets the declared constructors.
  /// </summary>
  /// <param name="type">The type.</param>
  /// <returns><![CDATA[IEnumerable<ConstructorInfo>]]></returns>
  public static IEnumerable<ConstructorInfo> GetDeclaredConstructors(this Type type)
  {
    return type.GetConstructors(InstanceFlags);
  }

  /// <summary>
  ///   Gets the field or property.
  /// </summary>
  /// <param name="type">The type.</param>
  /// <param name="name">The name.</param>
  /// <exception cref="ArgumentOutOfRangeException"></exception>
  /// <returns>A MemberInfo.</returns>
  public static MemberInfo GetFieldOrProperty(this Type type, string name)
  {
    return type.GetInheritedProperty(name) ?? (MemberInfo)type.GetInheritedField(name)
      ?? throw new ArgumentOutOfRangeException(nameof(name), $"Cannot find member {name} of type {type}.");
  }

  /// <summary>
  ///   Gets the generic interface.
  /// </summary>
  /// <param name="type">The type.</param>
  /// <param name="genericInterface">The generic interface.</param>
  /// <returns>A Type.</returns>
  public static Type GetGenericInterface(this Type type, Type genericInterface)
  {
    if (type.IsGenericType(genericInterface)) return type;

    foreach (var interfaceType in type.GetInterfacesCache())
      if (interfaceType.IsGenericType(genericInterface))
        return interfaceType;

    return null;
  }

  /// <summary>
  ///   Gets the i collection type.
  /// </summary>
  /// <param name="type">The type.</param>
  /// <returns>A Type.</returns>
  public static Type GetICollectionType(this Type type)
  {
    return type.GetGenericInterface(Metadata.ICollection1);
  }

  /// <summary>
  ///   Gets the i enumerable type.
  /// </summary>
  /// <param name="type">The type.</param>
  /// <returns>A Type.</returns>
  public static Type GetIEnumerableType(this Type type)
  {
    return type.GetGenericInterface(Metadata.IEnumerable1);
  }

  /// <summary>
  ///   Gets the inherited field.
  /// </summary>
  /// <param name="type">The type.</param>
  /// <param name="name">The name.</param>
  /// <returns>A FieldInfo.</returns>
  public static FieldInfo GetInheritedField(this Type type, string name)
  {
    return type.GetField(name, InstanceFlags) ?? type.BaseClassesAndInterfaces()
      .Select(t => t.GetField(name, InstanceFlags)).FirstOrDefault(f => f != null);
  }

  /// <summary>
  ///   Gets the inherited method.
  /// </summary>
  /// <param name="type">The type.</param>
  /// <param name="name">The name.</param>
  /// <exception cref="ArgumentOutOfRangeException"></exception>
  /// <returns>A MethodInfo.</returns>
  public static MethodInfo GetInheritedMethod(this Type type, string name)
  {
    return type.GetMethod(name, InstanceFlags)
           ?? type.BaseClassesAndInterfaces().Select(t => t.GetMethod(name, InstanceFlags))
             .FirstOrDefault(m => m != null) ?? throw new ArgumentOutOfRangeException(
             nameof(name),
             $"Cannot find member {name} of type {type}.");
  }

  /// <summary>
  ///   Gets the inherited property.
  /// </summary>
  /// <param name="type">The type.</param>
  /// <param name="name">The name.</param>
  /// <returns>A PropertyInfo.</returns>
  public static PropertyInfo GetInheritedProperty(this Type type, string name)
  {
    return type.GetProperty(name, InstanceFlags) ?? type.BaseClassesAndInterfaces()
      .Select(t => t.GetProperty(name, InstanceFlags)).FirstOrDefault(p => p != null);
  }

  /// <summary>
  ///   Gets the static method.
  /// </summary>
  /// <param name="type">The type.</param>
  /// <param name="name">The name.</param>
  /// <returns>A MethodInfo.</returns>
  public static MethodInfo GetStaticMethod(this Type type, string name)
  {
    return type.GetMethod(name, StaticFlags);
  }

  /// <summary>
  ///   Gets the type inheritance.
  /// </summary>
  /// <param name="type">The type.</param>
  /// <returns><![CDATA[IEnumerable<Type>]]></returns>
  public static IEnumerable<Type> GetTypeInheritance(this Type type)
  {
    while (type != null)
    {
      yield return type;

      type = type.BaseType;
    }
  }

  /// <summary>
  ///   Is collection.
  /// </summary>
  /// <param name="type">The type.</param>
  /// <returns>A bool.</returns>
  public static bool IsCollection(this Type type)
  {
    return type != Metadata<string>.Type && Metadata<IEnumerable>.Type.IsAssignableFrom(type);
  }

  /// <summary>
  ///   Is dynamic.
  /// </summary>
  /// <param name="type">The type.</param>
  /// <returns>A bool.</returns>
  public static bool IsDynamic(this Type type)
  {
    return Metadata<IDynamicMetaObjectProvider>.Type.IsAssignableFrom(type);
  }

  /// <summary>
  ///   Are the generic type.
  /// </summary>
  /// <param name="type">The type.</param>
  /// <param name="genericType">The generic type.</param>
  /// <returns>A bool.</returns>
  public static bool IsGenericType(this Type type, Type genericType)
  {
    return type.IsGenericType && type.GetGenericTypeDefinitionCache() == genericType;
  }

  /// <summary>
  ///   Are the list type.
  /// </summary>
  /// <param name="type">The type.</param>
  /// <returns>A bool.</returns>
  public static bool IsListType(this Type type)
  {
    return Metadata<IList>.Type.IsAssignableFrom(type);
  }

  /// <summary>
  ///   Are the nullable type.
  /// </summary>
  /// <param name="type">The type.</param>
  /// <returns>A bool.</returns>
  public static bool IsNullableType(this Type type)
  {
    return type.IsGenericType(Metadata.Nullable1);
  }

  /// <summary>
  ///   Statics the generic method.
  /// </summary>
  /// <param name="type">The type.</param>
  /// <param name="methodName">The method name.</param>
  /// <param name="parametersCount">The parameters count.</param>
  /// <exception cref="ArgumentOutOfRangeException"></exception>
  /// <returns>A MethodInfo.</returns>
  public static MethodInfo StaticGenericMethod(this Type type, string methodName, int parametersCount)
  {
    foreach (MethodInfo foundMethod in type.GetMember(
               methodName,
               MemberTypes.Method,
               StaticFlags & ~BindingFlags.NonPublic))
      if (foundMethod.IsGenericMethodDefinition && foundMethod.GetParameters().Length == parametersCount)
        return foundMethod;

    throw new ArgumentOutOfRangeException(
      nameof(methodName),
      $"Cannot find suitable method {type}.{methodName}({parametersCount} parameters).");
  }
}