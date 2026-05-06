using GC = Backend.GlobalConstants;
using Microsoft.Extensions.Caching.Memory;

/// <summary>
/// Load organisation configurations on startup
/// Created: June 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Config
{
    public class OrgConfigInitialiseService : BaseService, OrgConfigInitialiseServiceI
    {
        private readonly IMemoryCache _memoryCache;

        public OrgConfigInitialiseService(IServiceProvider serviceProvider,
            IMemoryCache memoryCache)
            : base(serviceProvider)
        {
            _memoryCache = memoryCache;
        }


        /// <summary>
        /// Load all orgainsational configurations and cache 
        /// </summary>
        /// <returns></returns>
        public async Task InitialiseOrgConfigs()
        {
            var orgs = await GetOrgs();

            var langCodes = new List<LangCode>();
            await Sql.Run(
                    "SELECT * FROM base.LangCode",
                    r => {
                        langCodes.Add(new LangCode()
                        {
                            Code = GetCode(r),
                            IsActive = IsActive(r),
                        });
                    }
                );

            //Config all organisations
            foreach (var org in orgs)
            {
                //Config languages
                var lConfigs = new List<LanguageConfig>();
                foreach (var l in org.Encoding.Languages)
                {
                    var v = ValidateLanguage(org, l, langCodes);
                    if (v != null) lConfigs.Add(v);
                }

                //Config Org
                var oConfig = new OrgConfig()
                {
                    orgNr = org.Nr,
                    LangCodeDefault = org.LangCode,
                    Languages = lConfigs
                };

                _memoryCache.Set(GC.CacheKeyOrgConfigPrefix + org.Nr, oConfig);
            }
        }

        private async Task<List<OrgEnt>> GetOrgs()
        {
            var list = new List<OrgEnt>();
            await Sql.Run(
                    "SELECT * FROM base.org ",
                    r => {
                        var org = OrgLoad.Load(r);
                        list.Add(org);
                        _memoryCache.Set(GC.CacheKeyOrgPrefix + org.Nr, org);
                    }
                );
            return list;
        }

        private LanguageConfig? ValidateLanguage(OrgEnt org, Language orgLang, List<LangCode> langCodes)
        {
            //Lang Code must exist in database
            var lc = langCodes.Find(c => c.Code == orgLang.LangCode);
            if (lc == null) return null;

            return new LanguageConfig()
            {
                LangCode = orgLang.LangCode,
                IsReadonly = orgLang.IsReadonly,
                IsEditable = orgLang.IsEditable,
            };
        }
    }
}
