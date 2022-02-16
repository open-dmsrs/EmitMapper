namespace EmitMapper.Utils;

using System;
using System.Collections.Generic;
using System.Linq;

public static class PrimitiveHelper
{
  public static void CheckIsDerivedFrom(this in TypesPair types, in TypesPair baseTypes)
  {
    types.SourceType.CheckIsDerivedFrom(baseTypes.SourceType);
    types.DestinationType.CheckIsDerivedFrom(baseTypes.DestinationType);
  }

  public static IEnumerable<T> Concat<T>(this IReadOnlyCollection<T> collection, IReadOnlyCollection<T> otherCollection)
  {
    otherCollection ??= Array.Empty<T>();
    if (collection.Count == 0) return otherCollection;
    return otherCollection.Count == 0 ? collection : Enumerable.Concat(collection, otherCollection);
  }

  public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
  {
    dictionary.TryGetValue(key, out var value);
    return value;
  }

  public static bool IsEnumToEnum(this in TypesPair context)
  {
    return context.SourceType.IsEnum && context.DestinationType.IsEnum;
  }

  public static bool IsEnumToUnderlyingType(this in TypesPair context)
  {
    return context.SourceType.IsEnum
           && context.DestinationType.IsAssignableFrom(Enum.GetUnderlyingType(context.SourceType));
  }

  public static bool IsUnderlyingTypeToEnum(this in TypesPair context)
  {
    return context.DestinationType.IsEnum
           && context.SourceType.IsAssignableFrom(Enum.GetUnderlyingType(context.DestinationType));
  }

  public static IReadOnlyCollection<T> NullCheck<T>(this IReadOnlyCollection<T> source)
  {
    return source ?? Array.Empty<T>();
  }
}