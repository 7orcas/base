namespace Backend.Program
{
    public class ZControllers
    {
        static public void Configure(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers(options =>
            {
                // options.Filters.Add<InterceptorFilter>();
            });
            //builder.Services.AddScoped<InterceptorFilter>();

        }
    }
}
