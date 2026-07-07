

using Backend.Base.Email;

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
        private readonly LoginRepoI _loginRepo;
        private readonly LabelServiceI _labelService;
        private readonly OrgServiceI _orgService;
        private readonly EmailServiceI _emailService;

        public SignupService(IServiceProvider serviceProvider,
            LoginServiceI loginService,
            LoginRepoI loginRepo,
            LabelServiceI labelService,
            OrgServiceI orgService,
            EmailServiceI emailService)
            : base(serviceProvider)
        {
            _loginService = loginService;
            _loginRepo = loginRepo;
            _labelService = labelService;
            _orgService = orgService;
            _emailService = emailService;
        }

        /*
         * Check the signup request and send an email for verification
         * Returns:
         * - success message
         * - error message (if attempt is still valid)
         * - emtpy string if suspicious (and logged)
         */
        public async Task<(bool success, string message)> SignupUser(string ipaddress, string username, string email, string password, int orgNr, string langCode)
        {
            var labels = await _labelService.GetLangCodeDic(langCode, null);
            var org = await _orgService.GetOrg(orgNr);

            //Check org
            if (org == null
                || !org.IsActive
                || !org.SignupEnabled)
            {
                _log.Error("Invalid OrgNr in signup Ipaddress {ipaddress} OrgNr {orgNr} Username {username}", ipaddress, orgNr, username);
                return (false, "");
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
                Emailverified = false,
                OrgNrDefault = org.Nr,
                LangCode = langCode,
                IsActive = !org.EmailVerified
            };

            //Save and senf email
            if (!await _loginRepo.CreateSignup(login))
                return (false, GetLabel("Oops", labels));

            var m = new LabelMessage(labels)
                    .AddBr("SignUp1");

            var template = new SignupVerifyEmailAddress(org, login);
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
                || !org.SignupEnabled)
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

            login.Emailverified = true;
            login.IsActive = true;

            if (!await _loginRepo.UpdateSignup(login))
                return (false, "LABELME cant update");

            //ToDo Create account

            return (true, "LABELME done");
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
