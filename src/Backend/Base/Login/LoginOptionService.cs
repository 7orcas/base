using Microsoft.JSInterop.Infrastructure;
using Npgsql;

namespace Backend.Base.Login
{
    public class LoginOptionService : BaseService, LoginOptionServiceI
    {
        private readonly OrgServiceI _orgService;

        public LoginOptionService(IServiceProvider serviceProvider,
            OrgServiceI orgService)
            : base(serviceProvider)
        {
            _orgService = orgService;
        }

        //Login options for the passed in login nr
        public async Task<LoginOptionEnt> GetLoginOptions(int loginNr)
        {
            var loginOption = null as LoginOptionEnt;
            
            try
            {
                await Sql.Run(
                    "SELECT * FROM cntrl.loginoption " +
                        "WHERE loginnr = @loginnr ",
                    r =>
                    {
                        loginOption = new LoginOptionEnt
                        {
                            LoginNr = GetInt(r, "loginnr"),
                            OrgNrs = GetString(r, "orgnrs"),
                            LangCode = GetString(r, "langcode"),
                            LangLabelVariant = GetInt(r, "langlabelvariant"),
                            LangCodes = GetString(r, "langcodes"),
                            MFA = GetInt(r, "mfa"),
                            RememberMe = GetBoolean(r, "rememberme"),
                            Forgot = GetBoolean(r, "forgot"),
                            SelfRegistration = GetBoolean(r, "selfregistration"),
                            IsActive = GetBoolean(r, "isActive")
                        };
                    },
                    new NpgsqlParameter("@loginnr", loginNr)
                );
            }
            catch { }

            return loginOption;
        }


        //Initialise login options 
        public async Task<LoginOptionDto> InitialiseLoginOptions(LoginOptionEnt ent)
        {

            var orgs = new List<OrgDto>();
            var dto = new LoginOptionDto
            {
                LoginNr = ent.LoginNr,
                LangCode = ent.LangCode,
                Orgs = orgs,
                LangCodes = ent.LangCodes?.Split(',').ToList() ?? new List<string>(),
                MFA = ent.MFA,
                RememberMe = ent.RememberMe,
                Forgot = ent.Forgot,
                SelfRegistration = ent.SelfRegistration
            };

            foreach (var part in ent.OrgNrs.Split(","))
            {
                var o = await _orgService.GetOrg(int.Parse(part));
                dto.Orgs.Add (_orgService.Populate(o));
            }

            return dto;
        }

    }
}
