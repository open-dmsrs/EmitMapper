namespace EmitMapper.Utils;

using System;
using System.Collections.Generic;

internal class ThreadSaveCache
{
    private readonly Dictionary<string, object> _cache = new();

    public T Get<T>(string key, Func<object> getter)
    {
        lock (this._cache)
        {
            if (!this._cache.TryGetValue(key, out var value))
            {
                value = getter();
                this._cache[key] = value;
            }

            return (T)value;
        }
    }
}