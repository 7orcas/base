using GC = Backend.GlobalConstants;
using Microsoft.Extensions.Caching.Memory;

/// <summary>
/// Create and manage sessions containing relevant objects for the logged in user
/// Note a user can have multiple sessions open
/// Created: April 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Session
{
    public class SessionService: BaseService, SessionServiceI
    {
        private readonly IMemoryCache _memoryCache;

        public SessionService(IServiceProvider serviceProvider,
            IMemoryCache memoryCache)
            : base(serviceProvider)
        {
            _memoryCache = memoryCache;
        }

        public async Task<SessionEnt> CreateSession(UserAccountEnt userAccount, OrgEnt org, UserConfig userConfig, int sourceApp)
        {
            var key = userAccount.Userid + "-" + Guid.NewGuid().ToString();
            var ses = new SessionEnt
            {
                Key = key,
                UserAccount = userAccount,
                Org = org,
                UserConfig = userConfig,
                SourceApp = sourceApp,
            };

            _memoryCache.Set(Key(key), ses);
            _log.Information("CreateSession, key=" + key + ", LoginId=" + userAccount.Id + ", org id=" + org.Nr);
            return ses;
        }

        public async Task RemoveSession(string key)
        {
            var ses = GetSession(key);
            if (ses == null)
            {
                _log.Error("RemoveSession, no session, key=" + key);
                return;
            }

            _log.Information("RemoveSession, key=" + key + ", LoginId=" + ses.UserAccount.Id + ", org id=" + ses.Org.Nr);
            _memoryCache.Remove(Key(key));
        }

        public SessionEnt? GetSession(string key)
        {
            if (_memoryCache.TryGetValue(Key(key), out var cachedValue))
            {
                return cachedValue as SessionEnt;
            }
            return null;
        }

        private string Key(string key)
        {
            return GC.CacheKeySessionPrefix + key;
        }

    }
}
