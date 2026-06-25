using System.Net;
using GC = FrontendLogin.GlobalConstants;

var builder = WebApplication.CreateBuilder(args);

LoadAppSettings(builder);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddHttpClient(GC.HTTP_Client, client =>
{
    client.BaseAddress = new Uri(AppSettings.Urls.Api); // Adjust base URL to your backend
});

//Here for Remember Me cookie handling, as the cookie is set by the backend and we need to ensure it is included in requests
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient(GC.HTTP_Client)
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        UseCookies = true,
        CookieContainer = new CookieContainer()
    });

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

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");


app.MapPost("/post-login", async (HttpContext context) =>
{
    var form = await context.Request.ReadFormAsync();

    var tk = form["tk"].ToString();
    var open = form["open"].ToString();
    var urlLogin = form["urlLogin"].ToString();

    // ✅ redirect into Blazor with safe query or fragment
    context.Response.Redirect($"/?tk={tk}&open={open}");
});


app.Run();


void LoadAppSettings(WebApplicationBuilder builder)
{
    var urls = new AppUrls();
    urls.Api = builder.Configuration["Urls:Api"];
    urls.Login = builder.Configuration["Urls:Login"];
    AppSettings.Urls = urls;

}
