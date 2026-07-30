using System.Reflection;

namespace Backend.Program
{
    public class ZVersionApi
    {

        static public void Use(WebApplication app)
        {
            app.MapGet("/api/version", () =>
            {
                var version = Assembly.GetExecutingAssembly()
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    ?.InformationalVersion;

                return Results.Ok(new
                {
                    Version = version
                });
            })
            .AllowAnonymous();
        }
    }
}
