using Npgsql;
using Microsoft.Extensions.Caching.Memory;
using GC = Backend.GlobalConstants;

/// <summary>
/// Holds all language dictionaries
/// Cached objects: 
/// - Dictionary<string, LangLabel> language label objects
/// - Dictionary<string, string> language lables only
/// Created: June 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Label
{
    public class LabelService : BaseService, LabelServiceI
    {
        private readonly IMemoryCache cache;
        private const string LangCodesCacheKey = "C";
        private const string AllLabelsCacheKey = "A";
        private const string LoginLabelsCacheKey = "L";

        public LabelService(IServiceProvider serviceProvider,
            IMemoryCache memoryCache) 
            : base(serviceProvider)
        {
            cache = memoryCache;
        }

        public async Task<Dictionary<string, LangLabel>> GetLanguageLabelDic(SessionEnt session) =>
            await GetLanguageLabelDic(session.UserConfig.LangCodeCurrent, session.Org.LangLabelVariant);
        

        public async Task<Dictionary<string, LangLabel>> GetLanguageLabelDic(string langCode, int? variant)
        {
            var key = CacheKey(langCode, variant, AllLabelsCacheKey);
            var dic = cache.Get<Dictionary<string, LangLabel>>(key);
            if (dic != null) return dic;

            await GetLanguageLabelList(langCode, variant);
            return cache.Get<Dictionary<string, LangLabel>>(key);
        }

        public async Task<Dictionary<string, string>> GetLangCodeDic(SessionEnt session) =>
            await GetLangCodeDic(session.UserConfig.LangCodeCurrent, session.Org.LangLabelVariant);
        

        public async Task<Dictionary<string, string>> GetLangCodeDic(string langCode, int? variant)
        {
            var key = CacheKey(langCode, variant, LangCodesCacheKey);
            var dic = cache.Get<Dictionary<string, string>>(key);
            if (dic != null) return dic;

            var list = await GetLanguageLabelList(langCode, variant);
            var dicX = list.ToDictionary(x => x.LangKeyCode, x => x.Code);
            cache.Set(key, dicX);

            return dicX;
        }


        private async Task SetLanguageLabelDic(string langCode, int? variant, List<LangLabel> list, string type)
        {
            var key = CacheKey(langCode, variant, type);
            var dic = list.ToDictionary(x => x.LangKeyCode, x => x);
            cache.Set(key, dic);
        }

        private string CacheKey(string langCode, int? variant, string type) =>
            GC.CacheKeyLabelPrefix + langCode + (variant.HasValue ? variant : GC.LangLabelVariantDefault) + type;
        
        /*
         * Get language list for passed in language code (eg 'en')
         */
        public async Task<List<LangLabel>> GetLanguageLabelList(string langCode, int? variant)
        {
            var key = CacheKey(langCode, variant, AllLabelsCacheKey);
            var dic = cache.Get<Dictionary<string, LangLabel>>(key);
            if (dic != null) return dic.Values.ToList(); 

            var list = await GetLabelList(null, langCode, null, null, null);
            if (variant.HasValue)
            {
                try
                {
                    var dict = list.ToDictionary(x => x.LangKeyCode, x => x);

                    var listX = await GetLabelList(null, langCode, null, null, variant);
                    foreach (var label in listX) 
                        dict[label.LangKeyCode] = label;
                    
                    list = dict.Values.ToList();
                }
                catch 
                {
                    _log.Error("Can't get language variants");
                }
            }

            await SetLanguageLabelDic(langCode, variant, list, AllLabelsCacheKey);
            return list;
        }

        /*
         * Get language list for passed in language code (eg 'en') for login page only
         */
        public async Task<List<LangLabel>> GetLanguageLabelListForLogin(string langCode, int? variant)
        {
            var key = CacheKey(langCode, variant, LoginLabelsCacheKey);
            var dic = cache.Get<Dictionary<string, LangLabel>>(key);
            if (dic != null) return dic.Values.ToList();

            var list = await GetLabelList(null, langCode, null, null, null);

            // Filter the list to only include login-related keys
            var loginKeys = new HashSet<string>
            {
                "Login",
                "UserName",
                "UserNameM",
                "Email",
                "Org",
                "Lang",
                "RemMe",
                "ForgotUP",
                "SignUp",
                "SignUpFail",
                "SysA",
                "NotRobot",
                "NotRobotConfirm",
                "ChgPw",
                "PW",
                "PWc",
                "PWx",
                "PWn",
                "PWsw",
                "PWhr",
                "PWsr",
                "PWReset",
                "PWReset0",
                "PWReset1",
                "PWReset2",
                "PWReset3",
                "Cancel",
                "Send",
                "PWResetES",
                "Mfa1",
                "Mfa2",
                "Mfa3",
                "MfaEr",
                "Verify",
                "CaptchaS",
                "CaptchaE",
                "CaptchaR",
                "Close",
                "PWait"
            };

            list = list
                .Where(x => loginKeys.Contains(x.LangKeyCode))
                .ToList();

            if (variant.HasValue)
            {
                try
                {
                    var dict = list.ToDictionary(x => x.LangKeyCode, x => x);

                    var listX = await GetLabelList(null, langCode, null, null, variant);
                    foreach (var label in listX)
                        dict[label.LangKeyCode] = label;

                    list = dict.Values.ToList();
                }
                catch
                {
                    _log.Error("Can't get language variants");
                }
            }

            await SetLanguageLabelDic(langCode, variant, list, LoginLabelsCacheKey);
            return list;
        }

        /*
         * Get language label for passed in language code id
         */
        public async Task<LangLabel> GetLanguageLabel(int id)
        {
            List <LangLabel> list = await GetLabelList(id, null, null, null, null);
            if (list.Count == 1) return list[0];
            return null;
        }

        public async Task<List<LangLabel>> GetAllLanguageLabels() =>
            await GetLabelList(null, null, null, null, null);

        /**
         * Return related labels, ie for a given label key get all languages
         */
        public async Task<List<LangLabel>> GetRelatedLabels(string langKeyCode, List<string> langCodes) =>
            await GetLabelList(null, null, langKeyCode, langCodes, null);

        /*
         * Get language key for passed in language key code 
         */
        public async Task<LangKey> GetLanguageKey(string langKeyCode)
        {
            var k = null as LangKey;
            await Sql.Run(
                   "SELECT * FROM base.langKey WHERE code = @langKeyCode",
                   r => {
                       k = new LangKey {
                           Id = GetId(r),
                           Pack = GetStringNull(r, "pack"),
                           Code = GetCode(r),
                           Description = GetDescription(r),
                           Updated = GetUpdated(r),
                       };
                   },
                   new NpgsqlParameter("@langKeyCode", langKeyCode)
               );

            return k;
        }


        private async Task<List<LangLabel>> GetLabelList(
            int? id, 
            string? langCode,
            string? langKeyCode,
            List<string> langCodes,
            int? variant)
        {
            string sql = "SELECT l.id, l.langKeyId, l.langCode, k.code AS kCode, l.variant, l.code, l.tooltip, l.updated " +
                            "FROM base.langLabel l " +
                            "INNER JOIN base.langKey k ON k.id = l.langKeyId ";

            string sqlWhere = "";

            if (id != null)
                sqlWhere = "WHERE l.id = " + id + " ";
            else
            {
                sqlWhere = "WHERE l.variant " + (variant.HasValue ? " = " + variant : " IS NULL") + " ";

                if (!string.IsNullOrEmpty(langCode))
                    sqlWhere += "AND l.langCode = '" + langCode + "' ";

                else if (langCodes  != null)
                {
                    sqlWhere += "AND (";
                    for (int i = 0; i < langCodes.Count; i++)
                        sqlWhere += (i==0?"":" OR ") + "l.langCode = '" + langCodes[i] + "'";
                    sqlWhere += ") ";
                }

                if (!string.IsNullOrEmpty(langKeyCode))
                    sqlWhere += "AND k.code = '" + langKeyCode + "' ";

            }
            sql += sqlWhere;

            //TESTING
            //sql += " and k.code = 'Machines' ";

            //Step 1: get labels for passed in lang code and org
            var list = new List<LangLabel>();
            await Sql.Run(sql,
                    r => {
                        list.Add(new LangLabel()
                        {
                            Id = GetId(r),
                            LangKeyId = GetId(r, "langKeyId"),
                            LangKeyCode = GetString(r, "kCode"),
                            LangCode = GetString(r, "langCode"),
                            Variant = GetIntNull(r, "variant"),
                            Code = GetCode(r),
                            Tooltip = GetStringNull(r, "tooltip"),
                            Updated = GetUpdated(r)
                        });
                    }
            );

            return list;
        }

        public async Task<List<LangCode>> GetLangCodeList()
        {
            var list = new List<LangCode>();
            await Sql.Run(
                    "SELECT * FROM base.langcode ",
                    r => {
                        var lang = new LangCode();
                        lang.Id = GetId(r);
                        lang.Code = GetCode(r);
                        lang.Description = GetDescription(r);
                        lang.Flag = GetStringNull(r, "flag");
                        lang.IsActive = IsActive(r);
                        list.Add(lang);
                    }
                );
            return list;
        }


        public async Task<bool> SaveLabel(LangLabel label)
        {
            string sql = "";

            if (label.Id > 0)
                sql = "UPDATE  base.langLabel SET "
                    + "code = '" + label.Code + "', "
                    + (label.Tooltip != null ? "tooltip = '" + label.Tooltip + "'" : "Tooltip = NULL" ) + " "
                    + "WHERE id = " + label.Id;
            else
                sql = "INSERT INTO  base.langLabel "
                    + "(langKeyId, langCode, variant, code, tooltip) "
                    + " VALUES ("
                    + label.LangKeyId + ","
                    + "'" + label.LangCode + "',"
                    + (label.Variant != null ? label.Variant : "NULL") + ","
                    + "'" + label.Code + "',"
                    + (label.Tooltip != null ? "'" + label.Tooltip + "'" : "NULL")
                    + ")";

            await Sql.ExecuteAsync(sql);
            return true;
        }

    }
}
