namespace Backend.Program
{
    public class ZMiddleware
    {
        static public void Configure(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseMiddleware<ApiDevMiddleware>();
            }

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<SessionMiddleware>();
            app.UseMiddleware<AuthorizationMiddleware>();
            app.UseMiddleware<AuditMiddleware>();

        }
    }
}
