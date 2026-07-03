


using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Npgsql;

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
        private readonly LabelServiceI _labelService;
        private readonly OrgServiceI _orgService;
        private readonly EmailServiceI _emailService;

        public SignupService(IServiceProvider serviceProvider,
            LoginServiceI loginService,
            LabelServiceI labelService,
            OrgServiceI orgService,
            EmailServiceI emailService)
            : base(serviceProvider)
        {
            _loginService = loginService;
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
                _log.Warning("Invalid OrgNr in signup Ipaddress {ipaddress} OrgNr {orgNr} Username {username}", ipaddress, orgNr, username);
                return (false, "");
            }

            //Validate password
            var r0 = _orgService.ValidatePassword(password, org, labels);
            if (!r0.valid)
                return (r0.valid, r0.message);

            var r1 = IsValidUsername(username, labels);
            if (!r1.valid)
                return (r1.valid, r1.message);

            if (!IsValidEmail(email))
                return (false, GetLabel("EmailInvalid", labels));

            //Check unique user name
            try
            {
                long id = -987;
                await Sql.Run(
                    "SELECT id FROM base.zzz " +
                        "WHERE xxx = @userid ",
                    r =>
                    {
                        id = GetId(r);
                    },
                    new NpgsqlParameter("@userid", username)
                );

                if (id == -987)
                    return (false, GetLabel("UserNameEx", labels));
            }
            catch 
            {
                return (false, GetLabel("Oops", labels));
            }

            //Check unique email
            try
            {
                long id = -987;
                await Sql.Run(
                    "SELECT id FROM base.zzz " +
                        "WHERE email = @email ",
                    r =>
                    {
                        id = GetId(r);
                    },
                    new NpgsqlParameter("@email", email)
                );

                if (id == -987)
                    return (false, GetLabel("EmailEx", labels));
            }
            catch
            {
                return (false, GetLabel("Oops", labels));
            }

            return (true, "");
        }

        private (bool valid, string message) IsValidUsername(string username, Dictionary<string, string>? labels)
        {
            bool isValid = true;
            var m = "";
            if (labels != null)
                m = GetLabel("UserName", labels) + ": ";


            if (string.IsNullOrWhiteSpace(username))
            {
                if (labels != null)
                    m += GetLabel("Val1", labels);
                isValid = false;
            }
            
            if (username.Length > LoginEnt.UseridMaxLength)
            { 
                if (!isValid)
                    m += ", ";
            
                if (labels != null)
                    m += GetLabel("InvL", labels).Replace("%%", LoginEnt.UseridMaxLength.ToString());
                isValid = false;
            }

            return (isValid, m);


        }

    }
}
