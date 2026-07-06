using Backend.Base.Email;
using Backend.Base.Token.Ent;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Npgsql;
using Org.BouncyCastle.Asn1.Ocsp;
using System.IO;
using System.Reflection.Emit;
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
        private readonly LoginRepoI _loginRepo;
        private readonly PermissionServiceI _permissionService;
        private readonly TokenServiceI _tokenService;
        private readonly OrgServiceI _orgService;
        private readonly ConfigServiceI _configService;
        private readonly SessionServiceI _sessionService;
        private readonly LabelServiceI _labelService;
        private readonly TemplateServiceI _templateService;
        private readonly EmailServiceI _emailService;

        private AppServiceAccount ServiceAccount = AppSettings.ServiceAccount;

        public LoginService (IServiceProvider serviceProvider,
            LoginRepoI loginRepo,
            TokenServiceI tokenService,
            OrgServiceI orgService,
            ConfigServiceI configService,
            PermissionServiceI permissionService,
            SessionServiceI sessionService,
            LabelServiceI labelService,
            TemplateServiceI templateService,
            EmailServiceI emailService) 
            : base (serviceProvider)
        {
            _loginRepo = loginRepo;
            _tokenService = tokenService;
            _orgService = orgService;
            _configService = configService;
            _permissionService = permissionService;
            _sessionService = sessionService;
            _labelService = labelService;
            _templateService = templateService;
            _emailService = emailService;
        }

        // Get the user login and account details, validate the password
        // Return a tokenkey if valid and MFA is not required or MFA is enabled and validated 
        public async Task<LoginEnt> LoginUser(string ipAddress, string userid, string password, int orgNr, int sourceAppNr, string? langCode, bool mfaValid)
        {
            try
            {
                var login = await GetLoginByUserid(userid);
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
                var err = await ValidateUser(login, password, org);
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
                login.Response.MainUrl = AppSettings.Urls.Client;
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
        private async Task<LoginEnt?> GetLoginByUserid(string userid)
        {
            var login = null as LoginEnt;
            if (ServiceAccount != null && userid.Equals(ServiceAccount.UserId))
            {
                login = LoginEnt.GetServiceLogin();
                login.Password = ServiceAccount.UserPw;

                var loginX = await GetLoginById(login.Id);
                if (loginX != null)
                {
                    login.MfaEnabled = loginX.MfaEnabled;
                    login.MfaSecret = loginX.MfaSecret;
                }

                return login;
            }
            return await _loginRepo.GetLoginByUserid(userid);
        }

        public async Task<LoginEnt?> GetLoginByEmail(string email)
        {
            return await _loginRepo.GetLoginByEmail(email);
        }


        public async Task<LoginEnt?> GetLoginById(long id)
        {
            var login = await _loginRepo.GetLoginById(id);
            var isService = ServiceAccount != null && id == GC.ServiceLoginId;

            if (login == null && isService)
            {
                login = LoginEnt.GetServiceLogin();
                login.Email = ServiceAccount.UserEmail;
                login.Password = ServiceAccount.UserPw;
            }

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
                            OrgNr = GetOrgNr(r),
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
        
        private async Task<string> ValidateUser (LoginEnt login, string password, OrgEnt org)
        {
            var langCode = login != null && login.LangCode != null ? login.LangCode : org.LangCode;
            var labels = await _labelService.GetLangCodeDic(langCode, org.LangLabelVariant);

            if (login == null || login.Id == 0)
                return GetLabel("LoginUP", "Invalid Username and/or Password", labels);

            if (org.EmailVerified && (string.IsNullOrEmpty(login.Email) || !login.Emailverified))
                return GetLabel("EmailVL", "Your email address must be verified before you can login", labels);

            await IncrementAttempts(login);

            if (string.IsNullOrEmpty(password) || !password.Equals(login.Password))
                return GetLabel("LoginUP", "Invalid Username and/or Password", labels);

            if (login.Attempts > org.Encoding.MaxNumberLoginAttempts)
                return GetLabel("LoginXP", "Max Attempts have been reached", labels);

            if (!login.IsActive)
                return GetLabel("LoginIA", "Username is Inactive", labels);

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

            await Sql.ExecuteAsync(
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
            await Sql.ExecuteAsync(
                   "UPDATE base.zzz "
                   + "SET mfasecret = '" + key + "' "
                   + "WHERE id = " + id
               );
            return true;
        }

        public async Task<bool> EnableMfa(long id)
        {
            await Sql.ExecuteAsync(
                   "UPDATE base.zzz "
                   + "SET mfaenabled = true "
                   + "WHERE id = " + id
               );
            return true;
        }

        public async Task<bool> ResetRequest(string email, string ipAddress)
        {
            var login = await GetLoginByEmail(email);

            if (login == null || !login.IsActive)
            {
                var message = "Reset request by " + (login == null ? "non-existant" : "inactive");
                _log.Warning(message + " email {Email} ipAddress {ipAddress}", email, ipAddress);
                return false;
            }

            var org = await _orgService.GetOrg(login.OrgNrDefault);

            if (org == null 
                || !org.IsActive
                || !org.ForgotEnabled)
            {
                _log.Warning("Reset request invalid org, email {Email} ipAddress {ipAddress}", email, ipAddress);
                return false;
            }

            var tv = new TokenValues
            {
                IpAddress = ipAddress,
                Username = email,
                SessionKey = "NoSession",
                OrgNr = org.Nr,
            };

            var token = _tokenService.CreateResetRequestToken(tv);

            var langCode = GC.LangCodeDefault;
            if (login.LangCode != null) langCode = login.LangCode;
            else if (org.LangCode != null) langCode = org.LangCode;
            login.LangCode = langCode;

            var dic = await _labelService.GetLangCodeDic(langCode, org.LangLabelVariant);
            var subject = "Password Reset";
            if (dic.TryGetValue("PWReset", out var value))
                subject = value;

            var template = new ResetRequestEmail(org, login, token);
            await _emailService.SendEmailAsync(email, subject, template.RenderTemplate());
            
            return true;
        }

        /*
         * Returns:
         * - success message
         * - error message (if attempt is still valid)
         * - emtpy string if suspicious (and logged)
         */
        public async Task<(bool success, string message)> ResetAction(string password, string token, string ipAddress, int orgNr, string langCode)
        {
            var labels = await _labelService.GetLangCodeDic(langCode, null);
            var tv = _tokenService.DecodeToken(token);


            //Check org
            var org = await _orgService.GetOrg(orgNr);

            if (org == null
                || !org.IsActive
                || !org.ForgotEnabled)
            {
                var user = tv != null? tv.Username : "null";
                _log.Warning("Invalid OrgNr when resetting password OrgNr {orgNr} Username {username}", orgNr, user);
                return (false, "");
            }

            if (tv != null && tv.IpAddress != ipAddress)
            {
                _log.Warning("Ipaddress mismatch when resetting password ipAddress {ipAddress} token-ipAddress {token-ipAddress}", ipAddress, tv.IpAddress);
                return (false, "");
            }

            if (tv == null)
                return (false, GetLabel("PWResetErr1", labels));

            var login = await GetLoginByEmail(tv.Username);

            //TEST ONLY
            //var login = await GetLoginByEmail("xx123"); 

            if (login == null)
                return (false, "");

            if (!login.IsActive)
                return (false, GetLabel("PWResetErr2", labels)); 

            if (login.OrgNrDefault != orgNr)
            {
                _log.Warning("OrgNr mismatch when resetting password Userid {Userid} OrgNr {OrgNr}", login.Userid, orgNr);
                return (false, "");
            }

            var r0 = _orgService.ValidatePassword(password, org, labels);
            if (!r0.valid)
                return (r0.valid, r0.message);

            login.Password = password;

            await Sql.ExecuteAsync(
                  "UPDATE base.zzz "
                  + "SET yyy = '" + password + "' "
                  + "WHERE id = " + login.Id
              );

            return (true, GetLabel("PWResetRsp", labels));
        }

    }
}
