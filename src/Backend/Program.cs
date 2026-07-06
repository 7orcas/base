using Backend;
using Backend.App.Machines;
using Backend.Base.DataProtection;
using Backend.Base.Email;
using Backend.Base.Registration;
using Backend.Base.Token.Ent;
using Backend.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens; // For TokenValidationParameters
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using GC = Backend.GlobalConstants;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog configuration
// Set up Serilog to use appsettings.json
builder.Logging.ClearProviders();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) // Reads from appsettings.json
    .Enrich.FromLogContext() //wrap log entries with additional context info (e.g. request details)
    .CreateLogger();

builder.Host.UseSerilog();

LoadAppSettings(builder);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add<InterceptorFilter>();
});
//builder.Services.AddScoped<InterceptorFilter>();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DBMainConnection")));


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true; // Prevent client-side access
    options.Cookie.IsEssential = true; // Mark the session cookie as essential
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Base API",
        Version = "v1"
    });

    // Define Bearer scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    // Require it
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorLogin",
        policy =>
        {
            policy.WithOrigins(AppSettings.CorsAllowedOrigins) 
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); 
        });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = TokenParameters.GetParameters());

/*  This must change when deploying to Azure */
builder.Services.AddDataProtection().SetApplicationName(GC.AppName);
/* For Azure Blob Storage key storage, add the following NuGet package:
   Microsoft.AspNetCore.DataProtection.AzureStorage
using Azure.Identity;
builder.Services.AddDataProtection()
    .PersistKeysToAzureBlobStorage(
        new Uri("https://<storage>.blob.core.windows.net/dpkeys/keys.xml"),
        new DefaultAzureCredential())
    .SetApplicationName(GC.AppName);
*/

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});

//Base Services (start up)
builder.Services.AddSingleton<OrgConfigInitialiseServiceI, OrgConfigInitialiseService>();
builder.Services.AddSingleton<PermissionInitialiseServiceI, PermissionInitialiseService>();

//Base Repos
builder.Services.AddScoped<LoginRepoI, LoginRepo>();
builder.Services.AddScoped<RoleRepoI, RoleRepo>();
builder.Services.AddScoped<TokenRepoI, TokenRepo>();


//Base Services
builder.Services.AddScoped<AuditServiceI, AuditService>();
builder.Services.AddScoped<LabelServiceI, LabelService>();
builder.Services.AddScoped<ConfigServiceI, ConfigService>();
builder.Services.AddScoped<LoginOptionServiceI, LoginOptionService>();
builder.Services.AddScoped<LoginServiceI, LoginService>();
builder.Services.AddScoped<SignupServiceI, SignupService>();
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


//App Services
builder.Services.AddScoped<MachineServiceI, MachineService>();

var app = builder.Build();

LogAppSettings(app);

app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestPath", httpContext.Request.Path);
        diagnosticContext.Set("RequestMethod", httpContext.Request.Method);
    };
});

app.UsePathBase(AppSettings.PathBase);

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseSession();
app.UseCors("AllowBlazorLogin");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //Note there is PathBase - needs to be recognised by swagger
    app.UseSwagger(c =>
        {
            c.PreSerializeFilters.Add((swagger, httpReq) =>
            {
                swagger.Servers = new List<OpenApiServer>
                {
                    new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}{AppSettings.PathBase}" }
                };
            });
        });

    app.UseSwaggerUI();
}

app.MapControllers();

app.Use(async (context, next) =>
{
    if (!context.Request.PathBase.Equals(AppSettings.PathBase))
    {
        context.Response.StatusCode = 404; // Not Found
        await context.Response.WriteAsync("Invalid path base.");
        return;
    }
    await next();
});

QuestPDF.Settings.License = LicenseType.Community;

RunOnStartup(app);
app.Run();



/// <summary>
/// Call the initialisation service methods
/// </summary>
void RunOnStartup(WebApplication app)
{
    var configService = app.Services.GetRequiredService<OrgConfigInitialiseServiceI>();
    configService.InitialiseOrgConfigs();

    var permissionService = app.Services.GetRequiredService<PermissionInitialiseServiceI>();
    permissionService.InitialisePermissions();
}

void LoadAppSettings(WebApplicationBuilder builder)
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
        acc.UserId = builder.Configuration["ServiceAccount:UserId"];
        acc.UserEmail = builder.Configuration["ServiceAccount:Email"];
        acc.UserPw = builder.Configuration["ServiceAccount:PW"];
        acc.AttemptsFile = builder.Configuration["ServiceAccount:AttemptsFile"];

        if (acc.IsValid())
            AppSettings.ServiceAccount = acc;
    }
    catch { }
}

void LogAppSettings(WebApplication app)
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