using Backend.App.DS.Machines;
using ds = Backend.App.DS.Base;

namespace Backend.Program.App.DS
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
            builder.Services.AddScoped<ds.Menu.MenuServiceI, ds.Menu.MenuService>();
            builder.Services.AddScoped<ds.Session.SessionServiceI, ds.Session.SessionService>();

            //INSERT_GENERATED_CODE_HERE

        }
    }
}
