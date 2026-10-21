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

        public async Task<TSession> CreateSession<TSession>(
            LoginAccountEnt userAccount, 
            OrgEnt org, 
            ConfigUser userConfig, 
            long? masqueradeId,
            int sourceApp, 
            string ipAddress, 
            string? sessionKey)
            where TSession : SessionEnt, new()
        {
            var key = sessionKey ?? userAccount.Username + "-" + Guid.NewGuid().ToString();
            var ses = new TSession
            {
                Key = key,
                Username = userAccount.Username,
                IsService = userAccount.IsService(),
                IsAdminUser = userAccount.IsAdminUser,
                IsAdminLang = userAccount.IsAdminLang,
                SourceApp = sourceApp,
                IpAddress = ipAddress,
                UserAccountId = userAccount.Id,
                MasqueradeId = masqueradeId,
                OrgNr = org.Nr,
                OrgNrDefault = userAccount.Login.OrgNrDefault,
                Classification = userAccount.Classification,
                LangCodeCurrent = userConfig.LangCodeCurrent,
                Languages = userConfig.Languages,
                Permissions = userAccount.Permissions,
            };

            _memoryCache.Set(Key(key), ses);
            _log.Information("CreateSession Key {SessionKey} OrgNr {OrgNr}", key, org.Nr);
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
            
            _log.Information("RemoveSession Key {SessionKey} LoginId {LoginId} Org {OrgId}",
                key, ses.UserAccountId, ses.Org.Nr);
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
