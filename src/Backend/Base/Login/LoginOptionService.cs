using Microsoft.Extensions.Options;
using Npgsql;
using GC = Backend.GlobalConstants;

namespace Backend.Base.Login
{
    public class LoginOptionService : BaseService, LoginOptionServiceI
    {
        private readonly OrgServiceI _orgService;
        private readonly LabelServiceI _labelService;

        public LoginOptionService(IServiceProvider serviceProvider,
            OrgServiceI orgService,
            LabelServiceI labelService)
            : base(serviceProvider)
        {
            _orgService = orgService;
            _labelService = labelService;
        }

        public async Task<LoginOptionEnt> GetLoginOptionDefault(int orgNr)
        {
            var loginOption = null as LoginOptionEnt;
            try
            {
                await Sql.Run(
                    "SELECT * FROM cntrl.loginoption " +
                        "WHERE orgnr = @orgNr " +
                        "AND isdefault IS TRUE",
                    r =>
                    {
                        loginOption = Load(r);
                    },
                    new NpgsqlParameter("@orgNr", orgNr)
                );
            }
            catch { }

            return loginOption;
        }


        //Login options for the passed in login nr
        public async Task<LoginOptionEnt> GetLoginOptions(string urlSuffix)
        {

            var whereSql = "urlsuffix = @urlsuffix";
            if (string.IsNullOrEmpty(urlSuffix))
            {
                whereSql = "isdefault = true";
                urlSuffix = "x";
            }
            else if (urlSuffix == GC.ServiceLoginUrlSuffix)
            {
                return await GetLoginOptionsService();
            }

            var loginOption = null as LoginOptionEnt;
            
            try
            {
                await Sql.Run(
                    "SELECT * FROM cntrl.loginoption " +
                        "WHERE " + whereSql,
                    r =>
                    {
                        loginOption = Load(r);
                    },
                    new NpgsqlParameter("@urlsuffix", urlSuffix)
                );
            }
            catch { }

            if (loginOption == null && !string.IsNullOrEmpty(urlSuffix))
                return await GetLoginOptions("");

            return loginOption;
        }

        //Make sure the options are enabled in at least one of the org numbers
        private async Task ReconcileOptionsWithOrgs(LoginOptionDto options)
        {
            var test = new LoginOptionDto
            {
                Mfa = true,
                RememberMe = false,
                Forgot = false,
                SelfRegistration = false,
                Masquerade = false,
            };

            foreach (var orgDto in options.Orgs)
            {
                var org = await _orgService.GetOrg(orgDto.Nr);
                ReconcileOptionsWithOrg(test, org);
            }

            if (!test.Mfa) options.Mfa = false;
            if (!test.RememberMe) options.RememberMe = false;
            if (!test.Forgot) options.Forgot = false;
            if (!test.SelfRegistration) options.SelfRegistration = false;
            if (!test.Masquerade) options.Masquerade = false;
        }

        private void ReconcileOptionsWithOrg(LoginOptionDto test, OrgEnt org)
        {
            if (org == null || !org.IsActive) return;

            if (org.Mfa > GC.MfaInactive) test.Mfa = true;
            if (org.IsRememberMeEnabled) test.RememberMe = true;
            if (org.IsMasqueradeEnabled) test.Masquerade = true;
            if (org.IsPasswordResetEnabled) test.Forgot = true;
            if (org.IsSignupEnabled) test.SelfRegistration = true;
        }


        static public LoginOptionEnt Load(NpgsqlDataReader r)
        {
            return new LoginOptionEnt
            {
                UrlSuffix = GetString(r, "urlsuffix"),
                OrgNr = GetOrgNr(r),
                OrgNrs = GetString(r, "orgnrs"),
                LangCode = GetString(r, "langcode"),
                LangLabelVariant = GetInt(r, "langlabelvariant"),
                LangCodes = GetString(r, "langcodes"),
                SuccessAction = GetInt(r, "successaction"),
                IsActive = GetBoolean(r, "isActive")
            };
        }


        private async Task<LoginOptionEnt> GetLoginOptionsService()
        {
            var orgList = await _orgService.GetOrgList();
            var orgs = string.Join(",", orgList.Select(o => o.Nr));

            var langCodeList = await _labelService.GetLangCodeList();
            var langs = string.Join(",", langCodeList.Select(l => l.Code));

            var loginOption = new LoginOptionEnt
            {
                OrgNrs = orgs,
                LangCode = GC.LangCodeDefault,
                LangLabelVariant = 0,
                LangCodes = langs,
                SuccessAction = GC.NavigateToFrontendServer,
                IsActive = true
            };
            
            return loginOption;
        }


        //Initialise login options 
        public async Task<LoginOptionDto> InitialiseLoginOptions(LoginOptionEnt ent)
        {
            //Get the default org for this login option
            var org = await _orgService.GetOrg(ent.OrgNr);
            var icon = string.Empty;
            if (!string.IsNullOrEmpty(org.Icon)) icon = org.Icon;

            var orgs = new List<OrgDto>();
            var langs = new List<LangCodeDto>();
            var dto = new LoginOptionDto
            {
                UrlSuffix = ent.UrlSuffix,
                Code = org.Code,
                Icon = icon,
                OrgNr = ent.OrgNr,
                LangCode = ent.LangCode,
                LangLabelVariant = ent.LangLabelVariant,
                Orgs = orgs,
                LangCodes = langs,
                SuccessAction = ent.SuccessAction,
                Mfa = org.Mfa > GC.MfaInactive,
                RememberMe = org.IsRememberMeEnabled,
                Forgot = org.IsPasswordResetEnabled,
                SelfRegistration = org.IsSignupEnabled,
                Masquerade = org.IsMasqueradeEnabled
            };

            foreach (var part in ent.OrgNrs.Split(","))
            {
                var o = await _orgService.GetOrg(int.Parse(part));
                if (!ent.IsService && !o.IsActive) continue;
                dto.Orgs.Add(new OrgDto
                {
                    Nr = o.Nr,
                    Code = o.Code,
                    Description = o.Description
                });

            }

            await ReconcileOptionsWithOrgs(dto);

            var list = await _labelService.GetLangCodeList();
            foreach (var part in ent.LangCodes.Split(","))
            {
                var l = list.Find(l => l.Code == part);
                if (l == null || (!ent.IsService && !l.IsActive)) continue;
                dto.LangCodes.Add(new LangCodeDto
                {
                    Code = l.Code,
                    Description = l.Description
                });

            }
            return dto;
        }

    }
}
