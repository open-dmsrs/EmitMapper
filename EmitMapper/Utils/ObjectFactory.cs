using System;
using System.Linq;
using System.Linq.Expressions;

namespace EmitMapper.Utils;

using static Expression;
using static ExpressionHelper;

public static class ObjectFactory
{
  private static readonly LazyConcurrentDictionary<Type, Func<object>> CtorCache = new();

  public static T CreateInstance<T>(Type type)
  {
    return (T)CreateInstance(type);
  }

  public static object CreateInstance(Type type)
  {
    return CtorCache.GetOrAdd(type, GenerateConstructor)();
  }

  public static object CreateInterfaceProxy(Type interfaceType)
  {
    return CreateInstance(ProxyGenerator.GetProxyType(interfaceType));
  }

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

  private static Expression CallConstructor(Type type)
  {
    var defaultCtor = type.GetConstructor(TypeExtensions.InstanceFlags, null, Type.EmptyTypes, null);
    if (defaultCtor != null) return New(defaultCtor);

    // find a ctor with only optional args
    var ctorWithOptionalArgs =
      type.GetDeclaredConstructors().FirstOrDefault(c => c.GetParameters().All(p => p.IsOptional));
    if (ctorWithOptionalArgs == null)
      return InvalidType(type, $"{type} needs to have a constructor with 0 args or only optional args.");

    // get all optional default values
    var args = ctorWithOptionalArgs.GetParameters().Select(p => ToType(p.GetDefaultValue(), p.ParameterType));

    // create the ctor expression
    return New(ctorWithOptionalArgs, args);
  }

  private static Expression CreateCollection(Type type, Type collectionType, Type[] genericArguments = null)
  {
    return ToType(New(collectionType.MakeGenericType(genericArguments ?? type.GenericTypeArguments)), type);
  }

  private static Expression CreateInterfaceExpression(Type type)
  {
    return type.IsGenericType(Metadata.IDictionary2) ? CreateCollection(type, Metadata.Dictionary2) :
      type.IsGenericType(Metadata.IReadOnlyDictionary2) ? CreateReadOnlyDictionary(type.GenericTypeArguments) :
      type.IsGenericType(Metadata.ISet1) ? CreateCollection(type, Metadata.HashSet1) :
      type.IsCollection() ? CreateCollection(type, Metadata.List1, GetIEnumerableArguments(type)) :
      InvalidType(type, $"Cannot create an instance of interface type {type}.");
  }

  private static Expression CreateReadOnlyDictionary(Type[] typeArguments)
  {
    var ctor = Metadata.ReadOnlyDictionary2.MakeGenericType(typeArguments).GetConstructors()[0];
    return New(ctor, New(Metadata.Dictionary2.MakeGenericType(typeArguments)));
  }

  private static Func<object> GenerateConstructor(Type type)
  {
    return Lambda<Func<object>>(GenerateConstructorExpression(type).ToObject()).Compile();
  }

  private static Type[] GetIEnumerableArguments(Type type)
  {
    return type.GetIEnumerableType()?.GenericTypeArguments ?? new[] { Metadata<object>.Type };
  }

  private static Expression InvalidType(Type type, string message)
  {
    return Throw(Constant(new ArgumentException(message, "type")), type);
  }
}