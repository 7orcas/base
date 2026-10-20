using Backend.App.DS.Machines;
using ds = Backend.App.DS.Base;
using hf = Backend.App.HF.Base;

namespace Backend.Program
{
    public class ZServicesApp
    {
        /// <summary>
        /// App Services
        /// </summary>
        /// <param name="builder"></param>
        static public void Configure(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<MachineServiceI, MachineService>();

            builder.Services.AddScoped<ds.Urls.UrlsServiceI, ds.Urls.UrlsService>();
            builder.Services.AddScoped<ds.Session.SessionServiceI, ds.Session.SessionService>();


            builder.Services.AddScoped<hf.Urls.UrlsServiceI, hf.Urls.UrlsService>();
            builder.Services.AddScoped<hf.Menu.MenuServiceI, hf.Menu.MenuService>();
            builder.Services.AddScoped<hf.Session.SessionServiceI, hf.Session.SessionService>();
        }
    }
}
