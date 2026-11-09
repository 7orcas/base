using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.Extensions.Caching.Memory;
using Npgsql;
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

        //DELETE ME delete refactored commented code below


        public LabelService(IServiceProvider serviceProvider,
            IMemoryCache memoryCache) 
            : base(serviceProvider)
        {
            if (AppSettings.Cache.Language)
                cache = memoryCache;
            
        }

        public async Task<Dictionary<string, LangLabel>> GetLanguageLabelDic(SessionEnt session) =>
            await GetLanguageLabelDic(session.LangCodeCurrent, session.Org.LangCodeVariant);
        

        public async Task<Dictionary<string, LangLabel>> GetLanguageLabelDic(string langCode, int? variant)
        {
            var key = CacheKey(langCode, variant, AllLabelsCacheKey);
            var dic = cache != null ? cache.Get<Dictionary<string, LangLabel>>(key) : null;
            if (dic != null) return dic;

            //await GetLanguageLabelList(langCode, variant);
            //return cache.Get<Dictionary<string, LangLabel>>(key);
            
            var list = await GetLanguageLabelList(langCode, variant);

            if (cache != null)
                return cache.Get<Dictionary<string, LangLabel>>(key);

            return list.ToDictionary(x => x.LangKeyCode, x => x);
        }

        public async Task<Dictionary<string, string>> GetLangCodeDic(SessionEnt session) =>
            await GetLangCodeDic(session.LangCodeCurrent, session.Org.LangCodeVariant);
        

        public async Task<Dictionary<string, string>> GetLangCodeDic(string langCode, int? variant)
        {
            var key = CacheKey(langCode, variant, LangCodesCacheKey);
            var dic = cache != null ? cache.Get<Dictionary<string, string>>(key) : null;
            if (dic != null) return dic;

            var list = await GetLanguageLabelList(langCode, variant);
            var dicX = list.ToDictionary(x => x.LangKeyCode, x => x.Code);

            if (cache != null)
                cache.Set(key, dicX);
            //return cache.Get<Dictionary<string, LangLabel>>(key);
            return dicX;
        }


        private async Task SetLanguageLabelDic(string langCode, int? variant, List<LangLabel> list, string type)
        {
            if (cache == null) return;
            var key = CacheKey(langCode, variant, type);
            var dic = list.ToDictionary(x => x.LangKeyCode, x => x);
            cache.Set(key, dic);
        }

        private string CacheKey(string langCode, int? variant, string type) =>
            GC.CacheKeyLabelPrefix + langCode + (variant.HasValue ? variant : GC.LangCodeVariantDefault) + type;
        
        /*
         * Get language list for passed in language code (eg 'en')
         */
        public async Task<List<LangLabel>> GetLanguageLabelList(string langCode, int? variant)
        {
            var key = CacheKey(langCode, variant, AllLabelsCacheKey);
            var dic = cache != null ? cache.Get<Dictionary<string, LangLabel>>(key) : null;
            if (dic != null) return dic.Values.ToList(); 

            var list = await GetLabelList(null, langCode, null, null, null);
            if (variant.HasValue && variant.Value > 0)
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
            var dic = cache != null ? cache.Get<Dictionary<string, LangLabel>>(key) : null;
            if (dic != null) return dic.Values.ToList();

            var list = await GetLabelList(null, langCode, null, null, null);
            list = FilterLoginLangKeys(list);

            if (variant.HasValue && variant.Value > 0)
            {
                var listNew = new List<LangLabel>();
                
                var listV = await GetLabelList(null, langCode, null, null, variant);
                listV = FilterLoginLangKeys(listV);

                foreach (var label in list)
                {
                    var labelV = listV.FirstOrDefault(x => x.LangKeyId == label.LangKeyId);
                    if (labelV != null)
                        listNew.Add(labelV);
                    else
                        listNew.Add(label);
                }

                list = listNew; 
            }

            await SetLanguageLabelDic(langCode, variant, list, LoginLabelsCacheKey);
            return list;
        }


        private List<LangLabel> FilterLoginLangKeys(List<LangLabel> list)
        {
            var loginKeys = new HashSet<string>
            {
                "Oops",
                "Cancel",
                "Close",
                "PWait",
                "Verify",
                "Login*",
                "UserName*",
                "Email",
                "Org",
                "Lang",
                "RemMe",
                "ForgotUP",
                "SignUp*",
                "SysA",
                "NotRobot*",
                "ChgPw",
                "PW*",
                "Send",
                "Mfa*",
                "Captcha*",
                "Ver"
            };

            list = list
                .Where(label => loginKeys.Any(key =>
                {
                    if (key.EndsWith("*"))
                    {
                        var prefix = key[..^1];
                        return label.LangKeyCode.StartsWith(
                            prefix,
                            StringComparison.OrdinalIgnoreCase);
                    }

                    return string.Equals(
                        label.LangKeyCode,
                        key,
                        StringComparison.OrdinalIgnoreCase);
                }))
                .ToList();

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
            string sql = "SELECT l.id, l.langKeyId, l.langCodeId, " +
                                "c.code AS langCode, c.variant, " +
                                "k.code AS kCode, " +
                                "l.code, l.updated " +
                            "FROM base.langLabel l " +
                            "JOIN base.langCode c ON c.id = l.langCodeId " +
                            "JOIN base.langKey k ON k.id = l.langKeyId ";

            string sqlWhere = "";

            if (id != null)
                sqlWhere = "WHERE l.id = " + id + " ";
            else
            {
                sqlWhere = "WHERE c.variant " + (variant.HasValue && variant.Value > 0 ? " = " + variant : " IS NULL") + " ";

                if (!string.IsNullOrEmpty(langCode))
                    sqlWhere += "AND c.code = '" + langCode + "' ";

                else if (langCodes  != null)
                {
                    sqlWhere += "AND (";
                    for (int i = 0; i < langCodes.Count; i++)
                        sqlWhere += (i==0?"":" OR ") + "c.code = '" + langCodes[i] + "'";
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
                            LangCodeId = GetId(r, "langCodeId"),
                            LangCode = GetString(r, "langCode"),
                            LangKeyId = GetId(r, "langKeyId"),
                            LangKeyCode = GetString(r, "kCode"),
                            Variant = GetIntNull(r, "variant"),
                            Code = GetCode(r),
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
                    + "WHERE id = " + label.Id;
            else
                sql = "INSERT INTO  base.langLabel "
                    + "(langKeyId, langCode, variant, code) "
                    + " VALUES ("
                    + label.LangKeyId + ","
                    + "'" + label.LangCode + "',"
                    + (label.Variant != null ? label.Variant : "NULL") + ","
                    + "'" + label.Code + "'"
                    + ")";

            await Sql.ExecuteAsync(sql);
            return true;
        }

    }
}
