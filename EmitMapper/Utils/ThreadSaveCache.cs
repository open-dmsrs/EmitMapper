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
            if
                /* Unmerged change from project 'EmitMapper (netstandard2.1)'
                Before:
                                if(!_cache.TryGetValue(key, out value))
                After:
                                if (!_cache.TryGetValue(key, out value))
                */
                (!this._cache.TryGetValue(key, out var value))
            {
                value = getter();
                this._cache[key] = value;
            }

            return (T)value;
        }
    }
}