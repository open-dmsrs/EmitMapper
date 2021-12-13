namespace EmitMapper.Utils;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

internal class ThreadSaveCache<T>
{
    private readonly ConcurrentDictionary<string, T> _cache = new();

    public T Get(string key, Func<T> getter)
    {
        if (!this._cache.TryGetValue(key, out T value))
        {
            value = getter();
            this._cache.TryAdd(key, value);
        }

        return value;
    }
}