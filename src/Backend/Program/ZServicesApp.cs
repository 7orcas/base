using Backend.App.Machines;

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
        }
    }
}
