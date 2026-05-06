using FrontendServer;
using GC = FrontendServer.GlobalConstants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using System.Net;
using FrontendServer.Base.Cache;
using FrontendServer.Base.Config;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog configuration
// Set up Serilog to use appsettings.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) // Reads from appsettings.json
    .CreateLogger();

builder.Host.UseSerilog();

LoadAppSettings(builder);

// Add services to the container.
builder.Services.AddMemoryCache();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

//ToDo is the ConfigurePrimaryHttpMessageHandler required?
builder.Services.AddHttpClient(GC.UnAuthorizedClientKey, client =>
{
    client.BaseAddress = new Uri(AppSettings.BackendApiBaseUri); 
}).ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    handler.UseCookies = true; // Enable cookies
    handler.CookieContainer = new CookieContainer(); // Cookie container for managing cookies
    return handler;
});

builder.Services.AddHttpClient(GC.AuthorizedClientKey, client =>
{
    client.BaseAddress = new Uri(AppSettings.AuthorizedClientBaseUri);
}); //.AddHttpMessageHandler<AuthorizationMessageHandler>();

//Base Services (start up)
builder.Services.AddSingleton<CacheInitialiseServiceI, CacheInitialiseService>();

// Add session services
builder.Services.AddSession();
builder.Services.AddSingleton<LabelCacheService>();
builder.Services.AddScoped<CacheService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<ProtectedSessionStorage>();

//Base Services 
builder.Services.AddScoped<HttpService>();
builder.Services.AddScoped<ConfigService>();
builder.Services.AddScoped<LabelService>();
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<BaseServiceDELETE_ME>();
builder.Services.AddScoped<LogoutService>();
builder.Services.AddScoped<PermissionService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<AuditService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
//app.UseSession(); // Add session services


app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

RunOnStartup(app);
app.Run();

void LoadAppSettings(WebApplicationBuilder builder)
{
    AppSettings.BackendApiBaseUri = builder.Configuration["Urls:BackendApiBaseUri"];
    AppSettings.AuthorizedClientBaseUri = builder.Configuration["Urls:AuthorizedClientBaseUri"];

 
}


/// <summary>
/// Call the initialisation service methods
/// </summary>
void RunOnStartup(WebApplication app)
{
    var cache = app.Services.GetRequiredService<CacheInitialiseServiceI>();
    cache.InitialiseCache();
}