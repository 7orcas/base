using Microsoft.Extensions.Caching.Memory;
using GC = Backend.GlobalConstants;

/// <summary>
/// Created: 
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Mfa
{
    public class MfaService : BaseService, MfaServiceI
    {
        private readonly IMemoryCache _memoryCache;

        public MfaService(IServiceProvider serviceProvider,
            IMemoryCache memoryCache) 
            : base(serviceProvider)
        {
            _memoryCache = memoryCache;
        }

        

    }
}
