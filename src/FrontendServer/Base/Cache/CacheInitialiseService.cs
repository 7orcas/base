using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Caching.Memory;
using GC = FrontendServer.GlobalConstants;

/// <summary>
/// Initialise the cache
/// Created: June 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace FrontendServer.Base.Cache
{
    public class CacheInitialiseService : CacheInitialiseServiceI
    {
        private readonly IMemoryCache _cache;

        public CacheInitialiseService(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }


        /// <summary>
        /// Reset cache
        /// </summary>
        /// <returns></returns>
        public async Task InitialiseCache()
        {
            //_cache.Remove(CacheService.LabelKey("en"));
            //_cache.Remove(CacheService.LabelKey("de"));
        }
    }
}
