using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace EmitMapper.Utils;

internal class ThreadSaveCache
{
    private static readonly ConcurrentDictionary<string, Tuple<Type, Func<object>>> _Cache = new();

    public Func<object> Get(string key, Func<string, Type> getter)
    {
        if (_Cache.TryGetValue(key, out var value))
            return value.Item2;

        var type = getter(key);
        var newItem = Tuple.Create(type, Expression.Lambda<Func<object>>(Expression.New(type)).Compile());
        _Cache.TryAdd(key, newItem);

        return newItem.Item2;
    }
}