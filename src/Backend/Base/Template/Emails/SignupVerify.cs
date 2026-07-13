using GC = Backend.GlobalConstants;

namespace Backend.Base.Template.Emails
{
    public class SignupVerify : BaseTemplate<SignupVerify>
    {
        private bool IsExpiry = false;
        private bool IsItSupport = false;

        public SignupVerify(OrgEnt org, LoginEnt login, Dictionary<string, string> labels) 
            : base(labels)
        {            
            OrgNr = org.Nr;
            
            if (login.LangCode != null) LangCode = login.LangCode;
            else if (org.LangCode != null) LangCode = org.LangCode;

            LangCodeVariant = org.LangLabelVariant;

            IsHtml = org.Encoding.IsEmailHtml;

            if (org.Encoding.SignupExpiryDays > 0)
            {
                IsExpiry = true;
                var expiry = DateTime.UtcNow.AddDays(org.Encoding.SignupExpiryDays);
                Data.Add("Expiry", expiry.ToString(org.Encoding.DateTimeFormatDMY));
            }

            if (!string.IsNullOrEmpty(org.Encoding.SupportIT))
            {
                IsItSupport = true;
                Data.Add("ITSupport", org.Encoding.SupportIT);
            }

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
            var t = @"<!DOCTYPE html>
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

                    XX1

                    <p>
                        If you did not create an account using this email address,
                        no further action is required. You may safely ignore this email.
                    </p>

                    <p>
                        If you require assistance, please contact the IT Support team
                        at [Support Contact Details].
                    </p>

                    XX2

                </body>
                </html>";

            if (IsExpiry)
                t = t.Replace("XX1",
               @"<p>
                    This verification link will expire on {{Expiry}}.
                    If the link expires before it is used, you will need to re-register.
                 </p>");
            else
                t = t.Replace("XX1", "");

            if (IsItSupport)
                t = t.Replace("XX2",
                               @"<p>
                    Kind regards,<br />
                    {{ITSupport}}
                 </p>");
            else
                t = t.Replace("XX2", "");

            return t;
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
