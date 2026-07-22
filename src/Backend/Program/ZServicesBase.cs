using Backend.Base.DataProtection;
using Backend.Base.Email;
using Backend.Base.Registration;

namespace Backend.Program
{
    public class ZServicesBase
    {
        static public void Configure(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<AuditServiceI, AuditService>();
            builder.Services.AddScoped<LabelServiceI, LabelService>();
            builder.Services.AddScoped<ConfigServiceI, ConfigService>();
            builder.Services.AddScoped<LoginOptionServiceI, LoginOptionService>();
            builder.Services.AddScoped<LoginServiceI, LoginService>();
            builder.Services.AddScoped<SignupServiceI, SignupService>();
            builder.Services.AddScoped<RobotServiceI, RobotService>();
            builder.Services.AddScoped<MfaServiceI, MfaService>();
            builder.Services.AddScoped<MfaKeyProtector>();
            builder.Services.AddScoped<CookieProtector>();
            builder.Services.AddScoped<RegistrationServiceI, RegistrationService>();
            builder.Services.AddScoped<TokenServiceI, TokenService>();
            builder.Services.AddScoped<OrgServiceI, OrgService>();
            builder.Services.AddScoped<SessionServiceI, SessionService>();
            builder.Services.AddScoped<PermissionServiceI, PermissionService>();
            builder.Services.AddScoped<RoleServiceI, RoleService>();
            builder.Services.AddScoped<EntityServiceI, EntityService>();
            builder.Services.AddScoped<TemplateServiceI, TemplateService>();
            builder.Services.AddScoped<PdfServiceI, PdfService>();
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddScoped<EmailServiceI, EmailService>();
            builder.Services.AddScoped<WordServiceI, WordService>();

        }

        /// <summary>
        /// Base Services (start up)
        /// </summary>
        /// <param name="builder"></param>
        static public void ConfigureStartup(WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<OrgConfigInitialiseServiceI, OrgConfigInitialiseService>();
            builder.Services.AddSingleton<PermissionInitialiseServiceI, PermissionInitialiseService>();

        }

        /// <summary>
        /// Call the initialisation service methods
        /// </summary>
        static public void RunOnStartup(WebApplication app)
        {
            var configService = app.Services.GetRequiredService<OrgConfigInitialiseServiceI>();
            configService.InitialiseOrgConfigs();

            var permissionService = app.Services.GetRequiredService<PermissionInitialiseServiceI>();
            permissionService.InitialisePermissions();
        }

    }
}
