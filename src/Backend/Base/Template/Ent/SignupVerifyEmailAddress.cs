using GC = Backend.GlobalConstants;

namespace Backend.Base.Template.Ent
{
    public class SignupVerifyEmailAddress : BaseTemplate<SignupVerifyEmailAddress>
    {
        public SignupVerifyEmailAddress(OrgEnt org, LoginEnt login) 
            : base()
        {            
            OrgNr = org.Nr;
            
            if (login.LangCode != null) LangCode = login.LangCode;
            else if (org.LangCode != null) LangCode = org.LangCode;

            LangCodeVariant = org.LangLabelVariant;

            IsHtml = org.Encoding.IsEmailHtml;

            Data.Add("Username", login.Username);
            Data.Add("Email", login.Email);
            Data.Add("OrgNr", org.Nr);
            Data.Add("VerifyLink", VerifyLink());
        }

        private string VerifyLink()
        {
            return AppSettings.Urls.Api 
                + GC.URL_signup_verify_email
                + "?OrgNr=" + OrgNr
                + "&LangCode=" + LangCode
                + "&LangVariant=" + LangCodeVariant
                + "&Email=" + Data["Email"];
        }

        protected override string Template()
        {
            if (LangCode == "de")
                return TemplateDe();
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
                    
                    Dear {{Username}},

                    <p>
                        Thank you for registering for an account.
                        To complete the registration process and activate your account,
                        please verify your email address by clicking the secure link below:
                    </p>

                    <p>
                        Verify Email Address: <a href=""{{VerifyLink}}"">Verify My Email</a>
                    </p>

                    <p>
                        Please note that this verification link will expire on
                        [Expiry Date and Time].
                        If the link expires before it is used, you will need to request
                        a new verification email.
                    </p>

                    <p>
                        If you did not create an account using this email address,
                        no further action is required. You may safely ignore this email.
                    </p>

                    <p>
                        If you require assistance, please contact the IT Support team
                        at [Support Contact Details].
                    </p>

                    <p>
                        Kind regards,<br />
                        IT Support Team
                    </p>

                </body>
                </html>";
        }

        private string TemplateDe()
        {
            return @"<!DOCTYPE html>
                <html>
                <head>
                    <meta charset=""utf-8"" />
                </head>
                <body style=""font-family: Arial;"">

                    ???

                </body>
                </html>";
        }

    }
}
