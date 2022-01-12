using System;
using System.Linq.Expressions;

namespace EmitMapper.Utils;

internal static class ThreadSaveCache
{
    private static readonly LazyConcurrentDictionary<string, ValueTuple<Type, Func<object>>> _Cache = new();

    public static Func<object> GetCreator(string key, Func<string, Type> getter)
    {
        return _Cache.GetOrAdd(
            key,
            _ =>
            {
                var type = getter(_);
                return ValueTuple.Create(type, Expression.Lambda<Func<object>>(Expression.New(type)).CompileFast());
            }).Item2;
    }

    public static ValueTuple<Type, Func<object>> Get(string key, Func<string, Type> getter)
    {
        return _Cache.GetOrAdd(
            key,
            _ =>
            {
                var type = getter(_);
                return ValueTuple.Create(type, Expression.Lambda<Func<object>>(Expression.New(type)).CompileFast());
            });
    }
}