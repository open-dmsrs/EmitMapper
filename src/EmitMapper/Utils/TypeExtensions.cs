using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace EmitMapper.Utils;

public static class TypeExtensions
{
  public const BindingFlags InstanceFlags =
    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

  public const BindingFlags StaticFlags =
    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

  public static IEnumerable<Type> BaseClassesAndInterfaces(this Type type)
  {
    var currentType = type;

    while ((currentType = currentType.BaseType) != null) yield return currentType;
    foreach (var interfaceType in type.GetInterfacesCache()) yield return interfaceType;
  }

  public static void CheckIsDerivedFrom(this Type derivedType, Type baseType)
  {
    if (!baseType.IsAssignableFrom(derivedType) && !derivedType.IsGenericTypeDefinition
                                                && !baseType.IsGenericTypeDefinition)
      throw new ArgumentOutOfRangeException(nameof(derivedType), $"{derivedType} is not derived from {baseType}.");
  }

  public static int GenericParametersCount(this Type type)
  {
    return type.GetTypeInfo().GenericTypeParameters.Length;
  }

  public static IEnumerable<ConstructorInfo> GetDeclaredConstructors(this Type type)
  {
    return type.GetConstructors(InstanceFlags);
  }

  public static MemberInfo GetFieldOrProperty(this Type type, string name)
  {
    return type.GetInheritedProperty(name) ?? (MemberInfo)type.GetInheritedField(name)
      ?? throw new ArgumentOutOfRangeException(nameof(name), $"Cannot find member {name} of type {type}.");
  }

  public static Type GetGenericInterface(this Type type, Type genericInterface)
  {
    if (type.IsGenericType(genericInterface)) return type;

    foreach (var interfaceType in type.GetInterfacesCache())
      if (interfaceType.IsGenericType(genericInterface))
        return interfaceType;

    return null;
  }

  public static Type GetICollectionType(this Type type)
  {
    return type.GetGenericInterface(Metadata.ICollection1);
  }

  public static Type GetIEnumerableType(this Type type)
  {
    return type.GetGenericInterface(Metadata.IEnumerable1);
  }

  public static FieldInfo GetInheritedField(this Type type, string name)
  {
    return type.GetField(name, InstanceFlags) ?? type.BaseClassesAndInterfaces()
      .Select(t => t.GetField(name, InstanceFlags)).FirstOrDefault(f => f != null);
  }

  public static MethodInfo GetInheritedMethod(this Type type, string name)
  {
    return type.GetMethod(name, InstanceFlags)
           ?? type.BaseClassesAndInterfaces().Select(t => t.GetMethod(name, InstanceFlags))
             .FirstOrDefault(m => m != null) ?? throw new ArgumentOutOfRangeException(
             nameof(name),
             $"Cannot find member {name} of type {type}.");
  }

  public static PropertyInfo GetInheritedProperty(this Type type, string name)
  {
    return type.GetProperty(name, InstanceFlags) ?? type.BaseClassesAndInterfaces()
      .Select(t => t.GetProperty(name, InstanceFlags)).FirstOrDefault(p => p != null);
  }

  public static MethodInfo GetStaticMethod(this Type type, string name)
  {
    return type.GetMethod(name, StaticFlags);
  }

  public static IEnumerable<Type> GetTypeInheritance(this Type type)
  {
    while (type != null)
    {
      yield return type;

      type = type.BaseType;
    }
  }

  public static bool IsCollection(this Type type)
  {
    return type != Metadata<string>.Type && Metadata<IEnumerable>.Type.IsAssignableFrom(type);
  }

  public static bool IsDynamic(this Type type)
  {
    return Metadata<IDynamicMetaObjectProvider>.Type.IsAssignableFrom(type);
  }

  public static bool IsGenericType(this Type type, Type genericType)
  {
    return type.IsGenericType && type.GetGenericTypeDefinitionCache() == genericType;
  }

  public static bool IsListType(this Type type)
  {
    return Metadata<IList>.Type.IsAssignableFrom(type);
  }

  public static bool IsNullableType(this Type type)
  {
    return type.IsGenericType(Metadata.Nullable1);
  }

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