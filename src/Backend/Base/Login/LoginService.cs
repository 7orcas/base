using Backend.Base.Email;
using Backend.Base.Token.Ent;
using System.Reflection.Metadata.Ecma335;
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
        private readonly IHostEnvironment _environment;
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

        public LoginService (
            IHostEnvironment environment,
            IServiceProvider serviceProvider,
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
            _environment = environment;
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
        public async Task<LoginEnt> LoginUser(string ipAddress, string username, string password, int orgNr, int sourceAppNr, string langCode, bool isMfaValid)
        {
            try
            {
                var loginErr = new LoginEnt();
                loginErr.Response.IsValid = false;
                //loginErr.Response.ErrorMessage = null; By default no message to avoid giving hints to hackers


                var login = await GetLoginByUsername(username);
                var org = await _orgService.GetOrg(orgNr);
                if (login == null || org == null)
                    return loginErr;


                var labels = await _labelService.GetLangCodeDic(langCode, org.LangLabelVariant);
                if (!await ValidateLogin(login, loginErr, password, org, labels))
                    return loginErr;


                UserAccountEnt? account = null;
                if (ServiceAccount != null && login.IsService())
                    account = UserAccountEnt.GetServiceAccount(orgNr);
                else
                    account = await _loginRepo.GetAccount(login.Id, orgNr);

                if (account == null)
                {
                    var label = GetLabel("LoginAS", "You are not setup to access %%", labels);
                    label = ReplaceLabelParameter(label, org.Code);
                    loginErr.Response.ErrorMessage = label;
                    return loginErr;
                }
                else if (!account.IsActive)
                {
                    var label = GetLabel("LoginIA", "Your account into %% is inactive", labels);
                    label = ReplaceLabelParameter(label, org.Code);
                    loginErr.Response.ErrorMessage = label;
                    return loginErr;
                }

                //Is Mfa required?
                var isMfaRequired = false;
                var daysSinceLastLogin = login.Lastlogin.HasValue ? (DateTime.Today - login.Lastlogin.Value.Date).TotalDays : 100;
                if (org.Mfa == GC.MfaRequiredEachLogin) isMfaRequired = true;
                if (org.Mfa == GC.MfaRequiredEachDay && daysSinceLastLogin > 0) isMfaRequired = true;
                if (org.Mfa == GC.MfaOptionalEachDay && daysSinceLastLogin > 0 && login.IsMfaRequired) isMfaRequired = true;
                if (login.IsService()) isMfaRequired = !_environment.IsDevelopment(); //Service account always requires MFA

                if (!isMfaValid && isMfaRequired)
                {
                    login.Response.IsValid = true;
                    login.Response.IsMfaRequired = true;
                    login.Response.IsMfaEnabled = login.IsMfaEnabled;
                    return login;
                }

                //Continue with login process and return tokenkey
                langCode = !string.IsNullOrEmpty(langCode) ? langCode : account.LangCode; //Delete me
                await InitialiseLogin(login, account, org, sourceAppNr);
                var userConfig = _configService.CreateUserConfig(account, org, langCode);
                var session = await _sessionService.CreateSession(account, org, userConfig, sourceAppNr, ipAddress);

                var tv = new TokenValues
                {
                    IpAddress = ipAddress,
                    Username = username,
                    SessionKey = session.Key,
                    OrgNr = orgNr,
                };

                var tokenKey = _tokenService.CreateJWToken(tv);
                
                login.Response.IsValid = true;
                login.Response.TokenKey = tokenKey;
                login.Response.MainUrl = AppSettings.Urls.Client;
                login.Response.LangCode = userConfig.LangCodeCurrent;
                login.Response.IsMfaRequired = isMfaRequired;
                login.Response.IsMfaEnabled = login.IsMfaEnabled;

                return login;
            }
            catch 
            {
                var login = new LoginEnt();
                login.Response.IsValid = false;
                login.Response.ErrorMessage = "Unknown Error";
                return login;
            }
        }

        //Authenicate the user
        private async Task<LoginEnt?> GetLoginByUsername(string username)
        {
            var login = null as LoginEnt;
            if (ServiceAccount != null && username.Equals(ServiceAccount.Username))
            {
                login = LoginEnt.GetServiceLogin();
                login.Password = ServiceAccount.UserPw;
                login.IsMfaRequired = true;
                GetAttemptsService(login);

                var loginX = await GetLoginById(login.Id);
                if (loginX != null)
                {
                    login.IsMfaEnabled = loginX.IsMfaEnabled;
                    login.MfaSecret = loginX.MfaSecret;
                }

                return login;
            }
            return await _loginRepo.GetLoginByUsername(username);
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


        private async Task<bool> ValidateLogin (LoginEnt login, LoginEnt loginErr, string password, OrgEnt org, Dictionary<string, string> labels)
        {

            //Check login exists
            if (login == null || login.Id == 0)
            {
                loginErr.Response.ErrorMessage = GetLabel("LoginUP", "Invalid Username and/or Password", labels);
                return false;
            }
            
            var validPassword = !string.IsNullOrEmpty(password)  && password.Equals(login.Password);
            

            //Check is user's email is verified if org requires it. 
            if (!login.IsService() 
                && org.IsEmailVerified 
                && (string.IsNullOrEmpty(login.Email) || !login.IsEmailVerified))
            {
                loginErr.Response.ErrorMessage = GetLabel("EmailVL", "Your email address must be verified before you can login", labels);
                return false;
            }

            //Inactive users cannot login. This is a hard rule.
            if (!login.IsActive)
            {
                loginErr.Response.ErrorMessage = GetLabel("LoginIA", "Username is Inactive", labels);
                return false;
            }

            var attempts = login.Attempts.HasValue ? login.Attempts.Value : 0;

            //Service attempts are a hard limit. No warnings are given.
            if (login.IsService() && attempts > 3)
            {
                await IncrementAttempts(login);
                loginErr.Response.ErrorMessage = GetLabel("LoginXPX", "Max Attempts have been reached", labels);
                return false;
            }

            var attemptsRule = org.Encoding.LoginAttemptRule;
            var isLockedOut = attemptsRule.LockoutAttempts > 0 && attempts > attemptsRule.LockoutAttempts;

            //Lockout is a hard limit, if set. The user can no longer login until the lockout is removed by an admin.
            if (isLockedOut)
            {
                await IncrementAttempts(login);
                loginErr.Response.ErrorMessage = GetLabel("LoginXPX", "Max Attempts have been reached, your login is locked", labels);
                loginErr.Response.ShowResetLink = attemptsRule.LockoutPasswordResetLink && org.IsPasswordResetEnabled;
                return false;
            }

            var isWarning = attemptsRule.WarningAttempts > 0 && !isLockedOut && attempts > attemptsRule.WarningAttempts;

            //Warning lockout is a hard limit until the warning lockout period has expired. 
            if (isWarning && 
                login.AttemptsLockout.HasValue &&
                attemptsRule.WarningLockoutMinutes > 0)
            {
                if (Now() < login.AttemptsLockout.Value)
                {
                    string formatted = login.AttemptsLockout.Value.AddMinutes(1).ToLocalTime().ToString(org.Encoding.DateTimeFormatDMY);
                    var label = GetLabel("LoginLO", "Your login is locked out until %%.", labels);
                    label = ReplaceLabelParameter(label, formatted);
                    loginErr.Response.ErrorMessage = label;
                    return false;
                }
            }


            //If valid password then set attempts to 0, if invalid increment attempts and check if the user has reached the hard limit or warning limit.
            if (!validPassword ) 
                attempts = await IncrementAttempts(login);
            else
                attempts = 0;


            //Recheck attempts after incrementing. If the user has reached the hard limit, they cannot login until an admin removes the lockout.
            isLockedOut = attemptsRule.LockoutAttempts > 0 && attempts > attemptsRule.LockoutAttempts;
            isWarning = attemptsRule.WarningAttempts > 0 && !isLockedOut && attempts > attemptsRule.WarningAttempts;


            //Lockout is a hard limit, if set. The user can no longer login until the lockout is removed by an admin.
            if (isLockedOut)
            {
                loginErr.Response.ErrorMessage = GetLabel("LoginXPX", "Max Attempts have been reached, your login is locked", labels);
                loginErr.Response.ShowResetLink = attemptsRule.LockoutPasswordResetLink && org.IsPasswordResetEnabled;
                return false;
            }

            //Warning attempts is a soft limit, if set. The user can still login but will be warned that they have reached the warning attempts.
            if (isWarning)
            {
                var label = GetLabel("LoginWP", "Warning have been reached %% attempts.", labels);
                label = ReplaceLabelParameter(label, attempts.ToString());

                if (attemptsRule.LockoutAttempts > 0)
                {
                    var label1 = GetLabel("LoginAX", "Max login attempts are %%.", labels);
                    label1 = ReplaceLabelParameter(label1, attemptsRule.LockoutAttempts.ToString());
                    label += " " + label1;
                }

                loginErr.Response.ErrorMessage = label;
                
                loginErr.Response.ShowResetLink = attemptsRule.WarningPasswordResetLink && org.IsPasswordResetEnabled;

                if (attemptsRule.WarningLockoutMinutes > 0)
                {
                    label = GetLabel("LoginWPX", "Your login is locked for %% minutes.", labels);
                    label = ReplaceLabelParameter(label, attemptsRule.WarningLockoutMinutes.ToString());
                    loginErr.Response.ErrorMessage += " " + label;
                    await _loginRepo.SetLockoutTimestamp(login.Id, DateTimeOffset.Now.AddMinutes(attemptsRule.WarningLockoutMinutes));
                }

                return false;
            }

            if (!validPassword)
            {
                loginErr.Response.ErrorMessage = GetLabel("LoginUP", "Invalid Username and/or Password", labels);
                return false;
            }

            return validPassword;
        }


        public async Task InitialiseLogin(LoginEnt login, UserAccountEnt account, OrgEnt org, int sourceAppNr)
        {
            if (login.Id == GC.ServiceLoginId)
                SetAttemptsService(0);
            else
            {
                await _loginRepo.SetAttempts(login.Id, 0);
                await _loginRepo.UpdateLastLogin(login.Id, account.Id);
            }

            account.Username = login.Username;
            account.Permissions = await _permissionService.LoadEffectivePermissionsInt(account.Id, org.Nr);
            _auditService.LogInOut(sourceAppNr, org.Nr, account.Id, GC.EntityTypeLogin);
        }

        private async Task<int> IncrementAttempts(LoginEnt l)
        {
            if (l.IsService())
                GetAttemptsService(l);
            else if (l.Attempts == null)
                l.Attempts = 0;

            l.Attempts++;

            if (l.IsService())
                SetAttemptsService(l.Attempts.Value);

            await _loginRepo.SetAttempts(l.Id, l.Attempts.Value);
            return l.Attempts.Value;
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
                || !org.IsPasswordResetEnabled)
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

            //Delete me???
            var langCode = GC.LangCodeDefault;
            if (login.LangCode != null) langCode = login.LangCode;
            else if (org.LangCode != null) langCode = org.LangCode;
            login.LangCode = langCode;

            var labels = await _labelService.GetLangCodeDic(langCode, org.LangLabelVariant);
            var subject = "Password Reset";
            if (labels.TryGetValue("PWReset", out var value))
                subject = value;

            var template = new ResetPasswordRequest(org, login, token, labels);
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
                || !org.IsPasswordResetEnabled)
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
                _log.Warning("OrgNr mismatch when resetting password Username {Username} OrgNr {OrgNr}", login.Username, orgNr);
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
