using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Cors.Infrastructure;
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
        private readonly OrgRepoI _orgRepo;
        private readonly LabelServiceI _labelService;
        private readonly IMemoryCache _memoryCache;

        public OrgService(IServiceProvider serviceProvider,
            OrgRepoI orgRepo,
            LabelServiceI labelService,
            IMemoryCache memoryCache) 
            : base(serviceProvider) 
        {
            _orgRepo = orgRepo;
            _labelService = labelService;
            _memoryCache = memoryCache;
        }

        public async Task<DefinitionDto> GetDefinition(SessionEnt session)
        {
            var validator = new OrgVal(session, this);
            return validator.GetDefinition();
        }

        public async Task<List<OrgEnt>> GetOrgList()
        {
            return await _orgRepo.GetList();
        }

        public OrgDto PopulateList(SessionEnt session, OrgEnt org)
        {
            OrgDto orgDto = new OrgDto()
            {
                Nr = org.Nr,
                Code = org.Code,
                Description = org.Description,
                IsActive = org.IsActive,
            };

            return orgDto;
        }

        public OrgDto Populate(SessionEnt session, OrgEnt org)
        {
            var enc = org.Encoding;
            var langDtos = new List<OrgLangDto>();
            foreach (var lang in enc.Languages)
            {
                langDtos.Add(new OrgLangDto
                {
                    LangCode = lang.LangCode,
                    IsReadonly = lang.IsVisible,
                    IsEditable = lang.IsEditable,
                });
            }

            var orgDto = new OrgDto
            {
                Nr = org.Nr,
                Code = org.Code,
                Description = org.Description,
                Icon = org.Icon,
                Updated = org.Updated,
                Version = org.Version,
                IsActive = org.IsActive,
                LangCode = org.LangCode,
                LangLabelVariant = org.LangLabelVariant,
                Mfa = org.Mfa,
                IsRememberMeEnabled = org.IsRememberMeEnabled,
                IsMasqueradeEnabled = org.IsMasqueradeEnabled,
                IsPasswordResetEnabled = org.IsPasswordResetEnabled,
                IsSignupEnabled = org.IsSignupEnabled,
                IsEmailRequired = org.IsEmailRequired,
                IsEmailHtml = org.IsEmailHtml,

                Languages = langDtos,

                PasswordRule = new PasswordRuleDto
                {
                    MinLength = enc.PasswordRule.MinLength,
                    MaxLength = enc.PasswordRule.MaxLength,
                    IsMixedCase = enc.PasswordRule.IsMixedCase,
                    IsNonLetter = enc.PasswordRule.IsSpecial,
                    IsNumber = enc.PasswordRule.IsNumber,
                },

                LoginAttemptRule = new LoginAttemptRuleDto
                {
                    WarningAttempts = enc.LoginAttemptRule.WarningAttempts,
                    LockoutAttempts = enc.LoginAttemptRule.LockoutAttempts,
                    WarningLockoutMinutes = enc.LoginAttemptRule.WarningLockoutMinutes,
                    LockoutPasswordResetLink = enc.LoginAttemptRule.LockoutPasswordResetLink,
                    WarningPasswordResetLink = enc.LoginAttemptRule.WarningPasswordResetLink,
                }
            };

            return orgDto;
        }

        public async Task<List<ValidationDto>> ValidateOrg(SessionEnt session, List<OrgDto> update)
        {
            var validator = new OrgVal(session, this);
            var vals = new List<ValidationDto>();

            foreach (var dto in update)
            {
                VersionI? version = null;
                if (dto.IsValidatable())
                    version = await _orgRepo.GetVersion(dto.Nr);
                else if (dto.IsDelete) continue;

                var val = validator.Validate(dto, version);
                if (val != null)
                    vals.Add(val);
            }

            return vals;
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

        public async Task<OrgEnt?> UpdateOrg(SessionEnt session, OrgDto orgDto)
        {
            //No action required
            if (orgDto.IsNew() && orgDto.IsDelete)
                return null;

            return await _orgRepo.Update(orgDto);
        }

        

        public async Task<string> GetPasswordRules(string langCode, int orgNr)
        {
            var org = await GetOrg(orgNr);
            var val = org.Encoding.PasswordRule;
            var labels = await _labelService.GetLangCodeDic(langCode, org.LangLabelVariant);

            var rules = "";
            if (val.MinLength > 0) rules += "<br>" + GetLabel("LenMin", labels) + "=" + val.MinLength;
            if (val.MaxLength > 0) rules += "<br>" + GetLabel("LenMax", labels) + "=" + val.MaxLength;
            if (val.IsMixedCase) rules += "<br>" + GetLabel("PWmc", labels);
            if (val.IsNumber) rules += "<br>" + GetLabel("PWNum", labels);
            if (val.IsSpecial) rules += "<br>" + GetLabel("PWSp", labels);

            if (!string.IsNullOrEmpty(rules))
                rules = rules.Substring("<br>".Length);

            return rules;
        }


        public (bool valid, string message) ValidatePassword(string pw, OrgEnt org, Dictionary<string, string>? labels)
        {
            var m = new BaseLabelValidation(labels)
             .Initialize("PW", ": ")
             .SetLabelsLowerCase();

            var val = org.Encoding.PasswordRule;

            if (string.IsNullOrEmpty(pw))
            {
                m.Add("Val0");
                return (false, m.GetMessage());
            }

            if (val.MinLength > 0 && pw.Length < val.MinLength)
                m.Add(GetLabel("LenMin", labels) + "=" + val.MinLength);
            
            if (val.MaxLength > 0 && pw.Length > val.MaxLength)
                m.Add(GetLabel("LenMax", labels) + "=" + val.MaxLength);
            
            if (val.IsMixedCase && (!pw.Any(char.IsUpper) || !pw.Any(char.IsLower)))
                m.Add("PWmc");
            
            if (val.IsNumber && !pw.Any(char.IsDigit))
                m.Add("PWNum");

            if (val.IsSpecial && !pw.Replace(" ", "").Any(c => !char.IsLetterOrDigit(c)))
                m.Add("PWSp");

            return (m.IsValid(), m.GetMessage());
        }


    }
}
