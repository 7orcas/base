namespace Backend.Program
{
    public class ZControllers
    {
        static public void Configure(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<CallActionFilter>();
                options.Filters.Add<VersionActionFilter>();
                options.Filters.Add<AuditActionFilter>();
            });
        }
    }
}
