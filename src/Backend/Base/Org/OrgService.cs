using Microsoft.Extensions.Caching.Memory;
using Npgsql;
using Superpower.Model;
using GC = Backend.GlobalConstants;

/// <summary>
/// Organisation methods
/// Created: March 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>


namespace Backend.Base.Org
{
    public class OrgService: BaseService, OrgServiceI
    {
        private readonly LabelServiceI _labelService;
        private readonly IMemoryCache _memoryCache;

        public OrgService(IServiceProvider serviceProvider,
            LabelServiceI labelService,
            IMemoryCache memoryCache) 
            : base(serviceProvider) 
        {
            _labelService = labelService;
            _memoryCache = memoryCache;
        }

        public async Task<List<OrgEnt>> GetOrgList()
        {
            var list = new List<OrgEnt>();
            await Sql.Run(
                    "SELECT * FROM base.org ",
                    r => {
                        var org = new OrgEnt();
                        org.Nr = GetInt(r, "nr");
                        org.Code = GetCode(r);
                        org.Description = GetDescription(r);
                        org.Updated = GetUpdated(r);
                        org.IsActive = IsActive(r);
                        list.Add(org);
                    }
                );
            return list;
        }

        public async Task<OrgEnt> GetOrg(int nr)
        {
            var org = _memoryCache.Get<OrgEnt>(GC.CacheKeyOrgPrefix + nr);
            if (org != null) return org;

            try
            {
                await Sql.Run(
                    "SELECT * FROM base.org "
                    + "WHERE nr = @nr ",
                    r => {
                        org = OrgLoad.Load(r);
                        _memoryCache.Set(GC.CacheKeyOrgPrefix + org.Nr, org);
                    },
                    new NpgsqlParameter("@nr", nr)
                );
                                
                return org;
            }
            catch 
            {
                return null;
            }
        }

        public async Task UpdateOrg(OrgEnt org)
        {
            org.Encode();
            await Sql.Execute(
                    "UPDATE base.org " +
                    "SET " +
                        Update("code", org.Code) +
                        Update("descr", org.Description) +
                        Update("encoded", org.Encoded) +
                        Update("updated", org.Updated) +
                        Update("isActive", org.IsActive) +
                        Update("mfaRequired", org.MfaRequired) +
                        Update("forgotenabled", org.Forgotenabled) +
                        Update("emailRequired", org.EmailRequired) +
                        Update("langCode", org.LangCode) +
                        NoComma(Update("langLabelVariant", org.LangLabelVariant)) +
                    " WHERE nr = " + org.Nr
            );
            _memoryCache.Set(GC.CacheKeyOrgPrefix + org.Nr, org);
        }

        public OrgDto Populate (OrgEnt org)
        {
            OrgDto orgDto = new OrgDto()
            {
                Nr = org.Nr,
                Code = org.Code,
                Description = org.Description,
                Updated = org.Updated,
                IsActive = org.IsActive,
                LangCode = org.LangCode,
                LangLabelVariant = org.LangLabelVariant,
            };

            return orgDto;
        }

        public async Task<string> GetPasswordRules(string langCode, int orgNr)
        {
            var org = await GetOrg(orgNr);
            var val = org.Encoding.PasswordRule;
            var labels = await _labelService.GetLangCodeDic(langCode, org.LangLabelVariant);

            var rules = "";
            if (val.MinLength > 0) rules += "<br>" + GetLabel("LenMin", labels) + "=" + val.MinLength;
            if (val.MaxLength > 0) rules += "<br>" + GetLabel("LenMax", labels) + "=" + val.MinLength;
            if (val.IsMixedCase) rules += "<br>" + GetLabel("PWmc", labels);
            if (val.IsNumber) rules += "<br>" + GetLabel("PWNum", labels);
            if (val.IsNonLetter) rules += "<br>" + GetLabel("PWNonLet", labels);

            if (!string.IsNullOrEmpty(rules))
                rules = rules.Substring("<br>".Length);

            return rules;
        }


        public bool ValidatePassword(string pw, OrgEnt org)
        {
            var val = org.Encoding.PasswordRule;

            if (string.IsNullOrEmpty(pw))
                return false;

            if (val.MinLength > 0 && pw.Length < val.MinLength) return false;
            if (val.MaxLength > 0 && pw.Length > val.MaxLength) return false;
            if (val.IsMixedCase && !pw.Any(char.IsUpper)) return false;
            if (val.IsMixedCase && !pw.Any(char.IsLower)) return false;
            if (val.IsNumber && !pw.Any(char.IsDigit)) return false;
            if (val.IsNonLetter && !pw.Any(c => !char.IsLetter(c))) return false;

            return true;
        }


    }
}
