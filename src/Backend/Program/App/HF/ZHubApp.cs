using hf = Backend.App.HF;
using GC = Backend.App.HF.GlobalConstantsHF;

namespace Backend.Program.App.HF
{
    public class ZHubApp
    {
        static public void Configure(WebApplication app)
        {
            app.MapHub<hf.Trail.TrailHub>("/" + GC.TrailProgressHub);
        }

    }
}
