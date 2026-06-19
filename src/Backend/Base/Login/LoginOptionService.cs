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
                        loginOption = new LoginOptionEnt
                        {
                            UrlSuffix = GetString(r, "urlsuffix"),
                            OrgNrs = GetString(r, "orgnrs"),
                            LangCode = GetString(r, "langcode"),
                            LangLabelVariant = GetInt(r, "langlabelvariant"),
                            LangCodes = GetString(r, "langcodes"),
                            MFA = GetInt(r, "mfa"),
                            SuccessAction = GetInt(r, "successaction"),
                            RememberMe = GetBoolean(r, "rememberme"),
                            Forgot = GetBoolean(r, "forgot"),
                            SelfRegistration = GetBoolean(r, "selfregistration"),
                            Masquerade = GetBoolean(r, "masquerade"),
                            IsActive = GetBoolean(r, "isActive")
                        };
                    },
                    new NpgsqlParameter("@urlsuffix", urlSuffix)
                );
            }
            catch { }

            if (loginOption == null && !string.IsNullOrEmpty(urlSuffix))
                return await GetLoginOptions("");

            return loginOption;
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
                MFA = GC.MFAactive,
                SuccessAction = GC.NavigateToFrontendServer,
                RememberMe = true,
                Forgot = true,
                SelfRegistration = true,
                Masquerade = true,
                IsActive = true
            };
            
            return loginOption;
        }


        //Initialise login options 
        public async Task<LoginOptionDto> InitialiseLoginOptions(LoginOptionEnt ent)
        {

            var orgs = new List<OrgDto>();
            var langs = new List<LangCodeDto>();
            var dto = new LoginOptionDto
            {
                UrlSuffix = ent.UrlSuffix,
                LangCode = ent.LangCode,
                Orgs = orgs,
                LangCodes = langs,
                MFA = ent.MFA,
                SuccessAction = ent.SuccessAction,
                RememberMe = ent.RememberMe,
                Forgot = ent.Forgot,
                SelfRegistration = ent.SelfRegistration,
                Masquerade= ent.Masquerade
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
