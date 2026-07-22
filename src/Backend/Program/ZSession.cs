namespace Backend.Program
{
    /// <summary>
    /// Session allows you to store data for a specific user between requests.
    /// </summary>
    public class ZSession
    {
        static public void Configure(WebApplicationBuilder builder)
        {
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
                options.Cookie.HttpOnly = true; // Prevent client-side access
                options.Cookie.IsEssential = true; // Mark the session cookie as essential
            });
        }

        static public void Use(WebApplication app)
        {
            app.UseSession();
        }
    }
}
