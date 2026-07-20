

using Backend.Base.Email;
using Backend.Base.Template.Forms;

/// <summary>
/// Manage self registration process for user
/// Created: July 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>
namespace Backend.Base.Login
{
    public class SignupService : BaseService, SignupServiceI
    {
        private readonly LoginServiceI _loginService;
        private readonly LoginOptionServiceI _loginOptionService;
        private readonly LoginRepoI _loginRepo;
        private readonly LabelServiceI _labelService;
        private readonly OrgServiceI _orgService;
        private readonly TokenServiceI _tokenService;
        private readonly EmailServiceI _emailService;

        public SignupService(IServiceProvider serviceProvider,
            LoginServiceI loginService,
            LoginOptionServiceI loginOptionService,
            LoginRepoI loginRepo,
            LabelServiceI labelService,
            OrgServiceI orgService,
            TokenServiceI tokenService,
            EmailServiceI emailService)
            : base(serviceProvider)
        {
            _loginService = loginService;
            _loginOptionService = loginOptionService;
            _loginRepo = loginRepo;
            _labelService = labelService;
            _orgService = orgService;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        /*
         * Check the signup request and send an email for verification
         * Returns:
         * - success message
         * - error message (if attempt is still valid)
         * - emtpy string if suspicious (and logged)
         */
        public async Task<(bool success, string message)> SignupUser(string ipaddress, string username, string email, string password, int orgNr, string langCode, string token)
        {
            var org = await _orgService.GetOrg(orgNr);
            var labels = await _labelService.GetLangCodeDic(langCode, org.LangLabelVariant);

            //Check org
            if (org == null
                || !org.IsActive
                || !org.IsSignupEnabled)
            {
                _log.Error("Invalid OrgNr in signup Ipaddress {ipaddress} OrgNr {orgNr} Username {username}", ipaddress, orgNr, username);
                return (false, "");
            }

            //Check token
            if (org.Encoding.IsSignupCaptchaEnabled)
            {
                var tv = _tokenService.DecodeToken(token);
                if (string.IsNullOrEmpty(tv.IpAddress) || tv.IpAddress != ipaddress)
                {
                    _log.Error("Invalid captcha token in signup Ipaddress {ipaddress} OrgNr {orgNr} Username {username}", ipaddress, orgNr, username);
                    return (false, "");
                }
            }

            var val = new BaseLabelValidation(labels);

            //Validate inputs
            var r0 = _orgService.ValidatePassword(password, org, labels);
            if (!r0.valid)
                val.AddBr(r0.message);

            var r1 = IsUsernameValid(username, labels);
            if (!r1.valid)
                val.AddBr(r1.message);

            r1 = IsEmailValid(email, labels);
            if (!r1.valid)
                val.AddBr(r1.message);

            //Check unique user name
            if (!await _loginRepo.IsUniqueUsername(username))
                val.AddBr("UserNameEx");
            
            //Check unique email
            if (!await _loginRepo.IsUniqueEmail(email))
                val.AddBr("EmailEx");


            if (!val.IsValid())
                return (false, val.GetMessage());

            var login = new LoginEnt
            {
                Username = username,
                Password = password,
                Email = email,
                IsEmailVerified = false,
                OrgNrDefault = org.Nr,
                LangCode = langCode,
                IsActive = true
            };

            //Save and senf email
            if (!await _loginRepo.CreateSignup(login))
                return (false, GetLabel("Oops", labels));

            var m = new LabelMessage(labels)
                    .AddBr("SignUp1");

            var template = new Template.Emails.SignupVerify(org, login, labels);
            await _emailService.SendEmailAsync(email, GetLabel("EmailV", labels), template.RenderTemplate());

            m.AddBr("SignUp2")
             .AddBr("SignUp3");

            return (true, m.GetMessage());
        }

        public async Task<(bool valid, string message)> VerifyEmail(string ipAddress, string email, int orgNr, string langCode)
        {
            var labels = await _labelService.GetLangCodeDic(langCode, null);
            var org = await _orgService.GetOrg(orgNr);

            //Check org
            if (org == null
                || !org.IsActive
                || !org.IsSignupEnabled)
            {
                _log.Error("Invalid OrgNr in VerifyEmail Ipaddress {ipaddress} OrgNr {orgNr} Email {email}", ipAddress, orgNr, email);
                return (false, "");
            }

            var login = await _loginRepo.GetLoginByEmail(email);

            if (login == null)
            {
                _log.Error("Invalid email in VerifyEmail Ipaddress {ipaddress} OrgNr {orgNr} Email {email}", ipAddress, orgNr, email);
                return (false, "");
            }

            if (login.IsEmailVerified)
            {
                _log.Warning("Email already verified, Ipaddress {ipaddress} OrgNr {orgNr} Email {email}", ipAddress, orgNr, email);
                return (false, GetLabel("EmailVSX", "Email address is already verified", labels));
            }

            if (org.Encoding.SignupExpiryDays > 0)
            {
                var expiry = login.Updated.AddDays(org.Encoding.SignupExpiryDays);
                if (expiry < DateTime.UtcNow)
                {
                    _log.Error("Expired action in VerifyEmail Ipaddress {ipaddress} OrgNr {orgNr} Email {email}", ipAddress, orgNr, email);
                    return (false, GetLabel("EmailVX", "Email verification has expired", labels));
                }
            }

            login.IsEmailVerified = true;

            if (!await _loginRepo.VerifySignup(login))
                return (false, GetLabel("Oops", "Something went wrong! Please see your system admin.", labels));


            //Create html form to display 
            var option = await _loginOptionService.GetLoginOptionDefault(orgNr);
            var template = new EmailAddressVerified(org, langCode, labels, option.UrlSuffix);
            
            return (true, template.RenderTemplate());
        }


        private (bool valid, string message) IsUsernameValid(string username, Dictionary<string, string>? labels)
        {
            var val = new BaseLabelValidation(labels)
             .Initialize("UserName", ": ")
             .SetLabelsLowerCase();
            

            if (string.IsNullOrWhiteSpace(username))
                val.Add("Val1");
            
            if (username.Contains(' '))
                val.Add("InvS");
            
            if (!username.Replace(" ", "").All(char.IsLetterOrDigit))
                val.Add("InvSp");

            if (username.Length > LoginEnt.UsernameMaxLength)
                val.Add("InvL", LoginEnt.UsernameMaxLength);

            return (val.IsValid(), val.GetMessage());
        }

        private (bool valid, string message) IsEmailValid(string email, Dictionary<string, string>? labels)
        {
            var val = new BaseLabelValidation(labels)
             .Initialize("Email", ": ")
             .SetLabelsLowerCase();

            email = email ?? "";

            if (string.IsNullOrWhiteSpace(email))
                val.Add("Val1");

            if (email.Contains(' '))
                val.Add("InvS");

            if (email.Length > LoginEnt.EmailMaxLength)
                val.Add("InvL", LoginEnt.EmailMaxLength);

            if (!IsEmailValid(email))
                val.Add("EmailInvalid");

            return (val.IsValid(), val.GetMessage());
        }

    }
}
