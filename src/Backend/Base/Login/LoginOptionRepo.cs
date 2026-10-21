using Npgsql;
using GC = Backend.GlobalConstants;

namespace Backend.Base.Login
{
    public class LoginOptionRepo : BaseRepo, LoginOptionRepoI
    {
        public LoginOptionRepo(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public async Task<LoginOptionEnt?> GetLoginOptions(string urlSuffix)
        {
            
            if (string.IsNullOrEmpty(urlSuffix))
                return await GetLoginOptionsDefault();
            

            var loginOption = null as LoginOptionEnt;

            await Sql.Run(
                "SELECT * FROM cntrl.loginoption " +
                    "WHERE urlsuffix = @urlsuffix",
                r =>
                {
                    loginOption = Load(r);
                },
                new NpgsqlParameter("@urlsuffix", urlSuffix)
            );

            return loginOption;
        }

        public async Task<LoginOptionEnt?> GetLoginOptions(int orgNr)
        {

            var loginOption = null as LoginOptionEnt;

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

            return loginOption;
        }

        private async Task<LoginOptionEnt?> GetLoginOptionsDefault()
        {
            var loginOption = null as LoginOptionEnt;

            await Sql.Run(
                "SELECT * FROM cntrl.loginoption " +
                    "WHERE isdefault = true",
                r =>
                {
                    loginOption = Load(r);
                }
            );
            return loginOption;
        }

        static public LoginOptionEnt Load(NpgsqlDataReader r)
        {
            return new LoginOptionEnt
            {
                UrlSuffix = GetString(r, "urlSuffix"),
                OrgNr = GetOrgNr(r),
                OrgNrs = GetString(r, "orgNrs"),
                LangCode = GetString(r, "langCode"),
                LangLabelVariant = GetInt(r, "langLabelVariant"),
                LangCodes = GetString(r, "langCodes"),
                SuccessAction = GetInt(r, "successAction"),
                IsMasqueradeEnabled = GetBoolean(r, "isMasqueradeEnabled"),
                IsActive = GetBoolean(r, "isActive")
            };
        }

    }
}
