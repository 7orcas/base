using hf = Backend.App.HF;

namespace Backend.Program.App.HF
{
    public class ZHubApp
    {
        static public void Configure(WebApplication app)
        {
            app.MapHub<hf.Trail.TrailHub>("/progressHub");
        }

    }
}
