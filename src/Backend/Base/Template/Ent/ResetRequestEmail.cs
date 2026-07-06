using GC = Backend.GlobalConstants;

namespace Backend.Base.Template.Ent
{
    public class ResetRequestEmail : BaseTemplate<ResetRequestEmail>
    {
        public string Token { get; set; }

        public ResetRequestEmail(OrgEnt org, LoginEnt login, string token) 
            : base()
        {            
            Token = token;
            OrgNr = org.Nr;
            
            if (login.LangCode != null) LangCode = login.LangCode;
            else if (org.LangCode != null) LangCode = org.LangCode;

            LangCodeVariant = org.LangLabelVariant;

            IsHtml = org.Encoding.IsEmailHtml;

            Data.Add("Userid", login.Userid);
            Data.Add("OrgNr", org.Nr);
            Data.Add("ResetLink", ResetLink());
        }

        private string ResetLink()
        {
            return AppSettings.Urls.Login 
                + GC.URL_login_reset
                + "?OrgNr=" + OrgNr
                + "&LangCode=" + LangCode
                + "&LangVariant=" + LangCodeVariant
                + "&token=" + Token;
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

        private string TemplateDe()
        {
            return @"<!DOCTYPE html>
                <html>
                <head>
                    <meta charset=""utf-8"" />
                </head>
                <body style=""font-family: Arial;"">

                    Sehr geehrte/r {{Userid}},
                    <p>
                    wir haben eine Anfrage zum Zurücksetzen des Passworts für Ihr Konto erhalten.<br>
                    Um mit dem Zurücksetzen Ihres Passworts fortzufahren, klicken Sie bitte auf den sicheren Link unten:<br>
                    </p>
                    <p>
                    Link zum Zurücksetzen des Passworts: <a href=""{{ResetLink}}"">Passwort zurücksetzen</a>
                    </p>
                    <p>
                    Bitte beachten Sie, dass dieser Link am [Ablaufdatum und -uhrzeit] abläuft.<br> 
                    Nach diesem Zeitpunkt müssen Sie eine neue Anfrage zum Zurücksetzen des Passworts stellen.
                    </p>
                    <p>
                    Falls Sie dieses Zurücksetzen des Passworts nicht angefordert haben, ignorieren Sie bitte diese E-Mail.<br>
                    Zu Ihrer Sicherheit empfehlen wir, dass Sie sich umgehend an das IT-Support-Team wenden, um diese Aktivität zu melden.<br>
                    Wenn Sie Unterstützung benötigen, zögern Sie bitte nicht, den IT-Support unter [Support-Kontaktdaten] zu kontaktieren.<br>
                    </p>

                    <p>
                    Mit freundlichen Grüßen,<br>
                    IT-Support-Team
                    </p>
                </body>
                </html>";
        }

    }
}
