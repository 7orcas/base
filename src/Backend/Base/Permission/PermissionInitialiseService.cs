using GC = Backend.GlobalConstants;
using Microsoft.Extensions.Caching.Memory;

/// <summary>
/// Loading permissions on startup
/// Created: March 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Permission
{
    public class PermissionInitialiseService: BaseService, PermissionInitialiseServiceI
    {
        private readonly IMemoryCache _memoryCache;
        
        public PermissionInitialiseService(IServiceProvider serviceProvider,
            IMemoryCache memoryCache)
            : base(serviceProvider)
        {
            _memoryCache = memoryCache;
        }


        /// <summary>
        /// Load all permissions and store 
        /// </summary>
        /// <returns></returns>
        public async void InitialisePermissions()
        {
            var dic = new Dictionary<int, PermissionEnt>(); //permission nr, entity

            for (int i=0; i<GC.Permissions.Length; i += 2)
            {
                var nr = (int)GC.Permissions[i];
                var langKey = (string)GC.Permissions[i + 1];

                dic.Add(nr, new PermissionEnt 
                { 
                    Nr = nr,
                    LangKey = langKey,
                });
            }

            _memoryCache.Set(GC.CacheKeyPermDic, dic);
        }

        /// <summary>
        /// Get all permissions 
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, PermissionEnt> GetPermissions()
        {
            if (_memoryCache.TryGetValue(GC.CacheKeyPermDic, out var cachedValue))
            {
                return cachedValue as Dictionary<int, PermissionEnt>;
            }
            return new Dictionary<int, PermissionEnt>();
        }


    }
}
