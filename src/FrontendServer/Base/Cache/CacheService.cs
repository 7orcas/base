using Microsoft.Extensions.Caching.Memory;
using GC = FrontendServer.GlobalConstants;

/// <summary>
/// Scoped service, however IMemoryCache is a singleton. 
/// Allows client to persist strings
/// ToDo: may want to make this as an explicit singleton.
/// Created: April 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace FrontendServer.Base.Cache
{
    public class CacheService
    {
        private readonly IMemoryCache _cache;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }


        public string? GetString(string key)
        {
            if (_cache.TryGetValue(key, out string s))
                return s;
            return null;
        }

        public void PutString(string key, string s) => _cache.Set(key, s);

        public void RemoveString(string key) => _cache.Remove(key);
        

    }
}
