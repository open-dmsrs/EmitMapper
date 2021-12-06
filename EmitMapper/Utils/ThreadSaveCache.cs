using System;
using System.Collections.Generic;

namespace EmitMapper.Utils
{
    internal class ThreadSaveCache
    {
        private readonly Dictionary<string, object> _cache = new Dictionary<string, object>();

        public T Get<T>(string key, Func<object> getter)
        {
            lock (_cache)
            {
                if
/* Unmerged change from project 'EmitMapper (netstandard2.1)'
Before:
                if(!_cache.TryGetValue(key, out value))
After:
                if (!_cache.TryGetValue(key, out value))
*/
(!_cache.TryGetValue(key, out object value))
                {
                    value = getter();
                    _cache[key] = value;
                }
                return (T)value;
            }
        }
    }
}
