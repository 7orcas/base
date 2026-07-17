using Microsoft.AspNetCore.Mvc;

namespace Backend.Base.Template.Forms
{
    public class EmailAddressVerified : BaseTemplate<EmailAddressVerified> 
    {
        public EmailAddressVerified(OrgEnt org, string langCode, Dictionary<string, string> labels, string urlSuffix)
            : base(labels)
        {
            OrgNr = org.Nr;
            LangCode = langCode;
            IsHtml = org.IsEmailHtml;

            Data.Add("Title", GetLabel("EmailVS", "Email address verified"));
            Data.Add("Message", GetLabel("EmailVM", "You are now able to log in"));
            Data.Add("LoginLink", LoginLink(urlSuffix));
            Data.Add("Login", GetLabel("Login", "Login"));
        }

        private string LoginLink(string urlSuffix)
        {
            return AppSettings.Urls.Login
                + (AppSettings.Urls.Login.EndsWith("/") ? "" : "/")
                + urlSuffix;
        }

        protected override string Template()
        {
            return TemplateEn();
        }

        private string TemplateEn()
        {
            return """
                <!DOCTYPE html>
                <html lang=""en"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>{{Title}}</title>
                    <style>
                        body {
                            margin: 0;
                            font-family: Arial, sans-serif;
                            background: #f4f6f8;
                            display: flex;
                            justify-content: center;
                            align-items: center;
                            height: 100vh;
                        }

                        .container {
                            text-align: center;
                            background: white;
                            padding: 40px;
                            border-radius: 12px;
                            box-shadow: 0 4px 12px rgba(0,0,0,0.1);
                        }

                        .icon {
                            font-size: 60px;
                            color: #28a745;
                            margin-bottom: 20px;
                        }

                        h1 {
                            margin: 0 0 10px;
                            color: #333;
                        }

                        p {
                            margin: 0;
                            color: #666;
                            font-size: 18px;
                        }
                    </style>
                </head>
                <body>
                    <div class=""container"">
        
                        <h1>{{Title}}</h1>
                        <p>{{Message}}: <a href="{{LoginLink}}">{{Login}}</a></p>
                    </div>
                </body>
                </html>
                """;
        }


    }
}
