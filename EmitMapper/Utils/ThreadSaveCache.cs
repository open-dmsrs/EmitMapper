using System;
using System.Linq.Expressions;

namespace EmitMapper.Utils;

internal static class ThreadSaveCache
{
  private static readonly LazyConcurrentDictionary<string, Tuple<Type, Func<object>>> _Cache = new();

  public static Func<object> GetCreator(string key, Func<string, Type> getter)
  {
    return _Cache.GetOrAdd(
      key,
      _ =>
      {
        var type = getter(_);
        return Tuple.Create(type, Expression.Lambda<Func<object>>(Expression.New(type)).Compile());
      }).Item2;
  }

  public static Tuple<Type, Func<object>> Get(string key, Func<string, Type> getter)
  {
    return _Cache.GetOrAdd(
      key,
      _ =>
      {
        var type = getter(_);
        return Tuple.Create(type, Expression.Lambda<Func<object>>(Expression.New(type)).Compile());
      });
  }
}