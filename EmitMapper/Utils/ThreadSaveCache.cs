using System;
using System.Collections.Concurrent;

namespace EmitMapper.Utils;

internal class ThreadSaveCache<T>
{
    private readonly ConcurrentDictionary<string, T> _cache = new();

    public T Get(string key, Func<T> getter)
    {
        if (!_cache.TryGetValue(key, out var value))
        {
            value = getter();
            _cache.TryAdd(key, value);
        }

        return value;
    }
}