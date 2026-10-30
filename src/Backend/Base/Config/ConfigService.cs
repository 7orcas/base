using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
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
        private readonly UserCacheServiceI _userCacheService;
        private readonly IMemoryCache _memoryCache;

        public ConfigService(IServiceProvider serviceProvider,
            IMemoryCache memoryCache,
            UserCacheServiceI cacheService) 
            : base(serviceProvider)
        {
            _memoryCache = memoryCache;
            _userCacheService = cacheService;
        }

        public async Task<DefinitionDto> GetDefinition(SessionEnt session)
        {
            var validator = new ConfigVal(session, this);
            return validator.GetDefinition();
        }

        /// <summary>
        /// Created at Login Time
        /// </summary>
        /// <param name="userAccount"></param>
        /// <param name="org"></param>
        /// <param name="requestedLangCode"></param>
        /// <returns></returns>
        public ConfigUser CreateUserConfig(LoginAccountEnt userAccount, OrgEnt org, string? requestedLangCode)
        {
            var orgConfig = _memoryCache.Get<ConfigOrg>(GC.CacheKeyOrgConfigPrefix + org.Nr);

            var userLangCode = ValidateLanguageCode.Validate(userAccount, orgConfig, requestedLangCode);

            var userConfig = new ConfigUser()
            {
                orgNr = org.Nr,
                LangCodeCurrent = userLangCode,
                Languages = CreateClientLanguages(userAccount, orgConfig, userLangCode),
            };

            return userConfig;
        }
               

        /// <summary>
        /// Get list of language codes the client is able to view and edit
        /// </summary>
        /// <param name="userAccount"></param>
        /// <param name="orgConfig"></param>
        /// <param name="userLangCode"></param>
        /// <returns></returns>
        private List<ConfigLanguage> CreateClientLanguages(LoginAccountEnt userAccount, ConfigOrg orgConfig, string userLangCode)
        {
            var langList = new List<ConfigLanguage>();

            if (!userAccount.IsAdmin) return langList;
            if (!userAccount.IsService() && !orgConfig.IsLangCodeEditable) return langList;

            foreach (var cl in orgConfig.Languages)
            {
                var up = userAccount.IsService();
                if (!up && cl.LangCode.Equals(userLangCode)) up = cl.IsEditable;

                langList.Add(new ConfigLanguage { 
                    LangCode = cl.LangCode,
                    IsVisible = userAccount.IsService() || cl.IsVisible,
                    IsEditable = up
                });
            }

            //Sort list
            langList = langList.OrderByDescending(x => x.LangCode).ToList();
            var dl = langList.Find(x => x.LangCode == userLangCode);
            
            var langListX = new List<ConfigLanguage>();
            if (dl != null) 
                langListX.Add(dl);

            foreach (var l in langList)
                if (l.LangCode != userLangCode)
                    langListX.Add(l);

            return langListX;
        }

        public async Task<ConfigDto> PopulateDto(SessionEnt session)
        {
            var org = session.Org;
            var orgConfig = _memoryCache.Get<ConfigOrg>(GC.CacheKeyOrgConfigPrefix + org.Nr);

            //Is there a cached version of the config for this user?
            var cachedConfigDto = await _userCacheService.GetCache<ConfigDto>(session, GetUserCacheKey(org.Nr));
            cachedConfigDto ??= ConfigDto.CreateEmpty();

            var langCodeCurrent = session.LangCodeCurrent;
            if (!string.IsNullOrEmpty(cachedConfigDto.Label.LangCode))
                langCodeCurrent = cachedConfigDto.Label.LangCode;
            langCodeCurrent = ValidateLanguageCode.Validate(session, orgConfig, langCodeCurrent);

            var langs = new List<LanguageConfigDto>();
            foreach (var lang in session.Languages)
            {
                langs.Add(new LanguageConfigDto
                {
                    LangCode = lang.LangCode,
                    IsUpdateable = lang.IsEditable,
                });
            }

            return new ConfigDto
            {
                UniqueUserId = session.UserAccountId + 987123564,
                UniqueSessionId = UniqueSessionId.GetId(),

                Org = new OrgConfigDto
                {
                    Nr = session.Org.Nr,
                    Code = session.Org.Code,
                    Description = session.Org.Description,
                    Icon = session.Org.Icon
                },

                User = new UserConfigDto
                {
                    Username = session.Username,
                    IsService = session.IsService,
                    IsAdmin = session.IsAdmin,
                },

                Languages = langs.ToArray(),
                Label = new LabelConfigDto
                {
                    LangCode = langCodeCurrent,
                    Variant = org.LangLabelVariant,
                    ShowTooltip = true,
                    IsAdmin = session.IsAdmin && cachedConfigDto.Label.IsAdmin,
                    HighlightNoKey = cachedConfigDto.Label.HighlightNoKey, 
                },
            };
        }

        public string GetUserCacheKey(int orgNr) => "UserConfig_" + orgNr;
        

    }
}
