using GC = Backend.GlobalConstants;

namespace Backend.Base.Template.Ent
{
    public class ResetRequestEmail : BaseTemplate<ResetRequestEmail>
    {
        public string Token { get; set; }

        public ResetRequestEmail() : base(GC.TemplateType.ResetRequestEmail)
        {            
        }

        public void initialise (OrgEnt org, LoginEnt login, string token)
        {
            Token = token;
            if (login.LangCode != null) LangCode = login.LangCode;
            else if (org.LangCode != null) LangCode = org.LangCode;
            IsHtml = org.Encoding.IsEmailHtml;

            Data.Add("Userid", login.Userid);
            Data.Add("OrgNr", org.Nr);
            Data.Add("ResetLink", ResetLink());
        }


        private string ResetLink()
        {
            return "https://localhost:7289?token=" + Token;
        }

        protected override string Template()
        {
            return TemplateEn();
        }

        private string TemplateEn()
        {
            return @"<!DOCTYPE html>
                <html>
                <head>
                    <meta charset=""utf-8"" />
                </head>
                <body style=""font-family: Arial;"">

                    Dear {{Userid}},
                    <p>
                    We have received a request to reset the password for your account.<br>
                    To proceed with resetting your password, please click the secure link below:<br>
                    </p>
                    <p>
                    Reset Password Link: <a href=""{{ResetLink}}"">Reset Password</a>
                    </p>
                    <p>
                    Please note that this link will expire on [Expiry Date and Time].<br> 
                    After this time, you will need to submit a new password reset request.
                    </p>
                    <p>
                    If you did not request this password reset, please ignore this email.<br>
                    For your security, we recommend that you contact the IT Support team immediately to report this activity.<br>
                    If you require any assistance, please do not hesitate to contact IT Support at [Support Contact Details].<br>
                    </p>

                    <p>
                    Kind regards,<br>
                    IT Support Team
                    </p>
                </body>
                </html>";
        }


    }
}
