using Backend.Base.Token.Ent;
using Npgsql;
using System.IO;
using GC = Backend.GlobalConstants;

/// <summary>
/// Manage login process for user
/// Note a user can have multiple sessions open
/// Created: April 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Login
{
    public class LoginService: BaseService, LoginServiceI
    {
        private readonly PermissionServiceI _permissionService;
        private readonly TokenServiceI _tokenService;
        private readonly OrgServiceI _orgService;
        private readonly ConfigServiceI _configService;
        private readonly SessionServiceI _sessionService;
        private AppServiceAccount ServiceAccount = AppSettings.ServiceAccount;

        public LoginService (IServiceProvider serviceProvider,
            TokenServiceI tokenService,
            OrgServiceI orgService,
            ConfigServiceI configService,
            PermissionServiceI permissionService,
            SessionServiceI sessionService) 
            : base (serviceProvider)
        {
            _tokenService = tokenService;
            _orgService = orgService;
            _configService = configService;
            _permissionService = permissionService;
            _sessionService = sessionService;
        }

        // Get the user login and account details, validate the password
        // Return a tokenkey if valid and MFA is not required or MFA is enabled and validated 
        public async Task<LoginEnt> LoginUser(string ipAddress, string userid, string password, int orgNr, int sourceAppNr, string? langCode, bool mfaValid)
        {
            try
            {
                var login = await GetLogin(userid);
                UserAccountEnt? account = null;

                if (login == null) 
                    login = new LoginEnt();
                else if (ServiceAccount != null && login.IsService())
                    account = UserAccountEnt.GetServiceAccount(orgNr);
                else
                    account = await GetAccount(login.Id, orgNr);

                if (!login.IsActive ||
                    account == null ||
                    !account.IsActive)
                {
                    return login;
                }

                var org = await _orgService.GetOrg(orgNr);
                var err = await Validate(login, password, org);
                if (err != null)
                {
                    login.Response.Valid = false;
                    login.Response.ErrorMessage = err;
                    return login;
                }

                //Return only that the user / pw / orgnr is valid.
                if (!mfaValid && org.MfaRequired)
                {
                    login.Response.Valid = true;
                    login.Response.MfaRequired = true;
                    login.Response.MfaEnabled = login.MfaEnabled;
                    return login;
                }

                //Continue with login process and return tokenkey
                langCode = !string.IsNullOrEmpty(langCode) ? langCode : account.LangCode;
                await InitialiseLogin(login, account, org, sourceAppNr);
                var userConfig = _configService.CreateUserConfig(account, org, langCode);
                var session = await _sessionService.CreateSession(account, org, userConfig, sourceAppNr);

                var tv = new TokenValues
                {
                    IpAddress = ipAddress,
                    Username = userid,
                    SessionKey = session.Key,
                    OrgNr = orgNr,
                };

                var tokenKey = _tokenService.CreateJWToken(tv);
                
                login.Response.Valid = true;
                login.Response.TokenKey = tokenKey;
                login.Response.MainUrl = AppSettings.MainClientUrl;
                login.Response.LangCode = userConfig.LangCodeCurrent;
                login.Response.MfaRequired = org.MfaRequired;
                login.Response.MfaEnabled = login.MfaEnabled;

                return login;
            }
            catch 
            {
                var login = new LoginEnt();
                login.Response.Valid = false;
                login.Response.ErrorMessage = "Unknown Error";
                return login;
            }
        }

        //Authenicate the user
        private async Task<LoginEnt?> GetLogin(string userid)
        {
            var login = null as LoginEnt;
            if (ServiceAccount != null && userid.Equals(ServiceAccount.UserId))
            {
                login = LoginEnt.GetServiceLogin();
                login.Password = ServiceAccount.UserPw;

                var loginX = await GetLogin(login.Id);
                if (loginX != null)
                {
                    login.MfaEnabled = loginX.MfaEnabled;
                    login.MfaSecret = loginX.MfaSecret;
                }

                return login;
            }

            long? id = null;
            try
            {
                //ToDo Log
                if (!ValidateParameter(userid))
                    throw new Exception();

                await Sql.Run(
                    "SELECT id FROM base.zzz " +
                        "WHERE xxx = @userid ",
                    r =>
                    {
                        id = GetId(r);
                    },
                    new NpgsqlParameter("@userid", userid)
                );

                if (id != null)
                    login = await GetLogin(id.Value);
            }
            catch { }

            return login;
        }
        

        public async Task<LoginEnt?> GetLogin(long id)
        {
            var login = null as LoginEnt;
            var isService = ServiceAccount != null && id == GC.ServiceLoginId;

            try
            {
                await Sql.Run(
                    "SELECT * FROM base.zzz " +
                        "WHERE id = @id ",
                    r =>
                    {
                        login = new LoginEnt
                        {
                            Id = GetId(r),
                            Userid = GetString(r, "xxx"),
                            Email = GetString(r, "email"),
                            Password = GetString(r, "yyy"),
                            Attempts = GetIntNull(r, "attempts"),
                            Lastlogin = GetDateTime(r, "lastlogin"),
                            IsActive = GetBoolean(r, "isActive"),
                            MfaSecret = GetStringNull(r, "mfasecret"),
                            MfaEnabled = GetBoolean(r, "mfaenabled"),
                        };
                    },
                    new NpgsqlParameter("@id", id)
                );

                if (login == null && isService)
                {
                    login = LoginEnt.GetServiceLogin();
                    login.Email = ServiceAccount.UserEmail;
                    login.Password = ServiceAccount.UserPw;
                }

            }
            catch { }

            return login;
        }


        private async Task<UserAccountEnt?> GetAccount(long loginId, int orgNr)
        {
            var account = null as UserAccountEnt;

            try
            {
                await Sql.Run(
                    "SELECT * FROM base.userAcc " +
                        "WHERE zzzId = @zzzId " +
                        "AND orgNr = @orgNr",
                    r =>
                    {
                        account = new UserAccountEnt
                        {
                            Id = GetId(r),
                            LoginId = GetId(r, "zzzId"),
                            orgNr = GetOrgNr(r),
                            LangCode = GetStringNull(r, "langCode"),
                            Lastlogin = GetDateTime(r, "lastlogin"),
                            IsActive = IsActive(r),
                            IsAdmin = GetBoolean(r, "isAdmin"),
                            Classification = GetIntNull(r, "classification")
                        };
                    },
                    new NpgsqlParameter("@zzzId", loginId),
                    new NpgsqlParameter("@orgNr", orgNr)
                );
            }
            catch { }

            return account;
        }

        //ToDo Language codes!
        private async Task<string> Validate (LoginEnt l, string password, OrgEnt org)
        {
            if (l == null || l.Id == 0)
                return "Invalid Username and/or Password.";

            await IncrementAttempts(l);

            if (string.IsNullOrEmpty(password) || !password.Equals(l.Password))
                return "Invalid Username and/or Password";
            
            if (l.Attempts > org.Encoding.MaxNumberLoginAttempts)
                return "Max Attempts";

            if (!l.IsActive)
                return "In active Login";

            return null;
        }


        public async Task InitialiseLogin(LoginEnt login, UserAccountEnt account, OrgEnt org, int sourceAppNr)
        {
            await SetAttempts(login.Id, 0);
            account.Userid = login.Userid;
            account.Permissions = await _permissionService.LoadEffectivePermissionsInt(account.Id, org.Nr);
            _auditService.LogInOut(sourceAppNr, org.Nr, account.Id, GC.EntityTypeLogin);
        }

        private async Task<bool> IncrementAttempts(LoginEnt l)
        {
            if (l.IsService())
                GetAttemptsService(l);
            else if (l.Attempts == null)
                l.Attempts = 0;

            l.Attempts++;

            if (l.IsService())
                return SetAttemptsService(l.Attempts.Value);

            return await SetAttempts(l.Id, l.Attempts.Value);
        }

        private async Task<bool> SetAttempts(long id, int attempts)
        {
            if (id == GC.ServiceLoginId)
                return SetAttemptsService(attempts);

            await Sql.Execute(
                   "UPDATE base.zzz "
                   + "SET Attempts = " + attempts + " "
                   + "WHERE id = " + id
               );
            return true;
        }

        private void GetAttemptsService(LoginEnt l)
        {
            int attempts = 0;
            try
            {
                string a = File.ReadAllText(ServiceAccount.AttemptsFile);
                attempts = int.Parse(a);
            }
            catch (Exception ex)
            {
                attempts = 0;
            }
            l.Attempts = attempts;
        }

        private bool SetAttemptsService(int attempts)
        {
            File.WriteAllText(ServiceAccount.AttemptsFile, "" + attempts);
            return true;
        }

        public async Task<bool> SetMfaKey(long id, string key)
        {
            await Sql.Execute(
                   "UPDATE base.zzz "
                   + "SET mfasecret = '" + key + "' "
                   + "WHERE id = " + id
               );
            return true;
        }

        public async Task<bool> EnableMfa(long id)
        {
            await Sql.Execute(
                   "UPDATE base.zzz "
                   + "SET mfaenabled = true "
                   + "WHERE id = " + id
               );
            return true;
        }

    }
}
