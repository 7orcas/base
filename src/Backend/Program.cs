using Backend.Base.Token.Ent;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using QuestPDF.Infrastructure;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

ZLogging.Configure(builder);
ZAppSettings.Configure(builder);
ZRateLimiter.Configure(builder);
ZControllers.Configure(builder);
ZDataBase.Configure(builder);
ZSession.Configure(builder);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache();
builder.Services.AddSignalR();

ZSwagger.Configure(builder);
ZCors.Configure(builder);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = TokenParameters.GetParameters());

ZDataProtection.Configure(builder);

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddHttpClient();

ZServicesBase.ConfigureStartup(builder);
ZReposBase.Configure(builder);
Backend.Program.App.HF.ZReposApp.Configure(builder);
ZServicesBase.Configure(builder);
Backend.Program.App.DS.ZServicesApp.Configure(builder);
Backend.Program.App.HF.ZServicesApp.Configure(builder);


var app = builder.Build();

ZAppSettings.Log(app);
ZAppSettings.UseBasePath(app);
app.UseRouting();
ZCors.Use(app);
ZRateLimiter.Use(app);
ZMiddleware.Configure(app);
ZLogging.ConfigureRequests(app);
ZVersionApi.Use(app);
Backend.Program.App.HF.ZHubApp.Configure(app);


if (!app.Environment.IsProduction())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthorization();

ZSession.Use(app);
ZSwagger.ConfigureRequests(app);

app.MapControllers();

ZAppSettings.BasePath(app);

QuestPDF.Settings.License = LicenseType.Community;

ZServicesBase.RunOnStartup(app);
app.Run();



