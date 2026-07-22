namespace Backend.Program
{
    public class ZCors
    {
        static public void Configure(WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowBlazorLogin",
                    policy =>
                    {
                        policy.WithOrigins(AppSettings.CorsAllowedOrigins)
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    });
            });
        }

        static public void Use(WebApplication app)
        {
            app.UseCors("AllowBlazorLogin");
        }

    }
}
