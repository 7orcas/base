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

            ReconcileOptionsWithOrgs(loginOption);

            return loginOption;
        }

        //Make sure the options are enabled in at least one of the org numbers
        private void ReconcileOptionsWithOrgs(LoginOptionEnt options)
        {
            var test = new LoginOptionEnt
            {
                IsMfa = true,
                IsRememberMe = false,
                IsForgot = false,
                IsSelfRegistration = false,
                IsMasquerade = false,
            };

            ReconcileOptionsWithOrg(test, options.OrgNr);

            var orgNrs = options.OrgNrs.Split(",");
            foreach (string nr in orgNrs)
            {
                if (int.TryParse(nr, out int orgNr))
                    ReconcileOptionsWithOrg(test, orgNr);
            }

            if (!test.IsMfa) options.IsMfa = false;
            if (!test.IsRememberMe) options.IsRememberMe = false;
            if (!test.IsForgot) options.IsForgot = false;
            if (!test.IsSelfRegistration) options.IsSelfRegistration = false;
            if (!test.IsMasquerade) options.IsMasquerade = false;
        }

        private async void ReconcileOptionsWithOrg(LoginOptionEnt test, int orgNr)
        {
            var org = await _orgService.GetOrg(orgNr);

            if (org == null || !org.IsActive) return;

            if (org.Mfa > GC.MfaInactive) test.IsMfa = true;
            if (org.IsRememberMeEnabled) test.IsRememberMe = true;
            if (org.IsMasqueradeEnabled) test.IsMasquerade = true;
            if (org.IsForgotEnabled) test.IsForgot = true;
            if (org.IsSignupEnabled) test.IsSelfRegistration = true;
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
                IsMfa = GetBoolean(r, "isMfa"),
                IsRememberMe = GetBoolean(r, "isRememberMe"),
                IsForgot = GetBoolean(r, "isForgot"),
                IsSelfRegistration = GetBoolean(r, "isSelfRegistration"),
                IsMasquerade = GetBoolean(r, "isMasquerade"),
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
                IsMfa = true,
                IsRememberMe = true,
                IsForgot = true,
                IsSelfRegistration = true,
                IsMasquerade = true,
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
                Mfa = ent.IsMfa,
                RememberMe = ent.IsRememberMe,
                Forgot = ent.IsForgot,
                SelfRegistration = ent.IsSelfRegistration,
                Masquerade = ent.IsMasquerade
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
