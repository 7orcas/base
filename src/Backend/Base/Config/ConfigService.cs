using Backend.Base.Config.Ent;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using GC = Backend.GlobalConstants;

/// <summary>
/// Organisational and User configurations
/// Controls processing and client displays
/// Created: 
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Config
{
    public class ConfigService : BaseService, ConfigServiceI
    {
        private readonly IMemoryCache _memoryCache;

        public ConfigService(IServiceProvider serviceProvider,
            IMemoryCache memoryCache) 
            : base(serviceProvider)
        {
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Created at Login Time
        /// </summary>
        /// <param name="userAccount"></param>
        /// <param name="org"></param>
        /// <param name="requestedLangCode"></param>
        /// <returns></returns>
        public UserConfig CreateUserConfig(UserAccountEnt userAccount, OrgEnt org, string? requestedLangCode)
        {
            var orgConfig = _memoryCache.Get<OrgConfig>(GC.CacheKeyOrgConfigPrefix + org.Nr);
            var userLangCode = ValidateLanguageCode(userAccount, orgConfig, requestedLangCode);

            var userConfig = new UserConfig()
            {
                orgNr = org.Nr,
                LangCodeCurrent = userLangCode,
                Languages = CreateClientLanguages(userAccount, orgConfig, userLangCode),
            };

            return userConfig;
        }

        /// <summary>
        /// Make sure the user is using a valid language code
        /// </summary>
        /// <param name="userAccount"></param>
        /// <param name="orgConfig"></param>
        /// <param name="requestedLangCode"></param>
        /// <returns></returns>
        private string ValidateLanguageCode(UserAccountEnt userAccount, OrgConfig orgConfig, string? requestedLangCode)
        {
            //Is user using the default org lang code?
            if (requestedLangCode == null) requestedLangCode = orgConfig.LangCodeDefault;
            if (requestedLangCode.Equals(orgConfig.LangCodeDefault)) return requestedLangCode;

            //Is the user's lang code used by the organisation (and active if required)?
            foreach (var lang in orgConfig.Languages)
                if (requestedLangCode.Equals(lang.LangCode))
                {
                    if (userAccount.IsService()) return requestedLangCode;
                    if (userAccount.IsActiveLanguageAdmin && lang.IsActive()) return requestedLangCode;
                }
                    
            return orgConfig.LangCodeDefault;
        }


        /// <summary>
        /// Get list of language codes the client is able to view and edit
        /// </summary>
        /// <param name="userAccount"></param>
        /// <param name="orgConfig"></param>
        /// <param name="userLangCode"></param>
        /// <returns></returns>
        private List<LanguageConfig> CreateClientLanguages(UserAccountEnt userAccount, OrgConfig orgConfig, string userLangCode)
        {
            var langList = new List<LanguageConfig>();

            if (!userAccount.IsLanguageAdmin()) return langList;
            if (!userAccount.IsService() && !orgConfig.IsLangCodeEditable) return langList;

            foreach (var cl in orgConfig.Languages)
            {
                var up = userAccount.IsService();
                if (!up && cl.LangCode.Equals(userLangCode)) up = cl.IsEditable;
                if (!up && userAccount.IsActiveLanguageAdmin) up = cl.IsEditable;

                langList.Add(new LanguageConfig { 
                    LangCode = cl.LangCode,
                    IsReadonly = userAccount.IsService() || cl.IsReadonly,
                    IsEditable = up
                });
            }

            //Sort list
            langList = langList.OrderByDescending(x => x.LangCode).ToList();
            var dl = langList.Find(x => x.LangCode == userLangCode);
            
            var langListX = new List<LanguageConfig>();
            if (dl != null) 
                langListX.Add(dl);

            foreach (var l in langList)
                if (l.LangCode != userLangCode)
                    langListX.Add(l);

            return langListX;
        }

    }
}
