using hf = Backend.App.HF.Base;

namespace Backend.Program.App.HF
{
    public class ZServicesApp
    {
        /// <summary>
        /// App Services
        /// </summary>
        /// <param name="builder"></param>
        static public void Configure(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<hf.Urls.UrlsServiceI, hf.Urls.UrlsService>();
            builder.Services.AddScoped<hf.Menu.MenuServiceI, hf.Menu.MenuService>();
            builder.Services.AddScoped<hf.Session.SessionServiceI, hf.Session.SessionService>();

            builder.Services.AddScoped<TrailServiceI, TrailService>();
            builder.Services.AddScoped<TrailGroupServiceI, TrailGroupService>();
            //INSERT_GENERATED_CODE_HERE
        }
    }
}
