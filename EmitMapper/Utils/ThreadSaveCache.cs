using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace EmitMapper.Utils;

internal static class ThreadSaveCache
{
    private static readonly ConcurrentDictionary<string, Tuple<Type, Func<object>>> _Cache = new();

    public static Func<object> GetCtor(string key, Func<string, Type> getter)
    {
        if (_Cache.TryGetValue(key, out var value))
            return value.Item2;

        var type = getter(key);
        var newItem = Tuple.Create(type, Expression.Lambda<Func<object>>(Expression.New(type)).Compile());
        if (_Cache.TryAdd(key, newItem))
            return newItem.Item2;
        else
            throw new Exception("重复编译类型，Key已经存在");
    }
    public static Tuple<Type, Func<object>> Get(string key, Func<string, Type> getter)
    {
        if (_Cache.TryGetValue(key, out var value))
            return value;

        var type = getter(key);
        var newItem = Tuple.Create(type, Expression.Lambda<Func<object>>(Expression.New(type)).Compile());
        if (_Cache.TryAdd(key, newItem))
            return newItem;
        else
            throw new Exception("重复编译类型，Key已经存在");
    }
}