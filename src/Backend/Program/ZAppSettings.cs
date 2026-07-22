namespace Backend.Program
{
    public class ZAppSettings
    {
        static public void Configure(WebApplicationBuilder builder)
        {
            AppSettings.DBMainConnection = builder.Configuration["ConnectionStrings:DBMainConnection"];
            AppSettings.MaxGetTokenCalls = int.Parse(builder.Configuration["Token:MaxGetTokenCalls"]);
            AppSettings.AccessTokenMinutes = int.Parse(builder.Configuration["Token:AccessTokenMinutes"]);
            AppSettings.RefreshTokenDays = int.Parse(builder.Configuration["Token:RefreshTokenDays"]);
            AppSettings.CacheExpirationAddSeconds = int.Parse(builder.Configuration["Token:CacheExpirationAddSeconds"]);
            AppSettings.CacheExpirationGetSeconds = int.Parse(builder.Configuration["Token:CacheExpirationGetSeconds"]);
            AppSettings.AuthenticatorAppName = builder.Configuration["Mfa:AuthenticatorAppName"];
            AppSettings.CorsAllowedOrigins = builder.Configuration["Cors:AllowedOrigins"];
            AppSettings.PathBase = builder.Configuration["PathBase"];

            try
            {
                var urls = new AppUrls();
                urls.Api = builder.Configuration["Urls:Api"];
                urls.Client = builder.Configuration["Urls:Client"];
                urls.Login = builder.Configuration["Urls:Login"];
                AppSettings.Urls = urls;
            }
            catch { }

            try
            {
                var email = new EmailSettings();
                email.SmtpServer = builder.Configuration["Email:SmtpServer"];
                email.Port = int.Parse(builder.Configuration["Email:Port"]);
                email.SenderName = builder.Configuration["Email:SenderName"];
                email.SenderEmail = builder.Configuration["Email:SenderEmail"];
                email.Username = builder.Configuration["Email:Username"];
                email.Password = builder.Configuration["Email:Password"];
                AppSettings.EmailSettings = email;
            }
            catch { }

            //Do not log details!
            try
            {
                var acc = new AppServiceAccount();
                acc.Username = builder.Configuration["ServiceAccount:Username"];
                acc.UserEmail = builder.Configuration["ServiceAccount:Email"];
                acc.UserPw = builder.Configuration["ServiceAccount:PW"];
                acc.AttemptsFile = builder.Configuration["ServiceAccount:AttemptsFile"];

                if (acc.IsValid())
                    AppSettings.ServiceAccount = acc;
            }
            catch { }

            try
            {
                var recaptcha = new ReCaptcha();
                recaptcha.SiteKey = builder.Configuration["ReCaptcha:SiteKey"];
                recaptcha.SecretKey = builder.Configuration["ReCaptcha:SecretKey"];
                AppSettings.ReCaptcha = recaptcha;
            }
            catch { }
        }
    

        static public void Log(WebApplication app)
        {
            var _log = Serilog.Log.Logger;
            _log.Information("---------Backend Startup------------");

            _log.Information("mtc {MaxGetTokenCalls} ceas {CacheExpirationAddSeconds} cegs {CacheExpirationGetSeconds} mcurl {MainClientUrl} bp {BasePath} saa {ServiceAccountActive}",
                AppSettings.MaxGetTokenCalls,
                AppSettings.CacheExpirationAddSeconds,
                AppSettings.CacheExpirationGetSeconds,
                AppSettings.Urls.Client,
                AppSettings.PathBase,
                (AppSettings.ServiceAccount != null ? "Yes" : "No"));

            if (app.Environment.IsDevelopment())
            {
                _log.Information("dbc {DBConnection}", AppSettings.DBMainConnection);
            }
        }

        static public void BasePath(WebApplication app)
        {
            app.Use(async (context, next) =>
            {
                if (!context.Request.PathBase.Equals(AppSettings.PathBase))
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound; 
                    await context.Response.WriteAsync("Invalid path base.");
                    return;
                }
                await next();
            });
        }

        static public void UseBasePath(WebApplication app)
        {
            app.UsePathBase(AppSettings.PathBase);
        }

    }
}