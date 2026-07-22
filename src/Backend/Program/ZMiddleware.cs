namespace Backend.Program
{
    public class ZMiddleware
    {
        static public void Configure(WebApplication app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<SessionMiddleware>();
            app.UseMiddleware<AuthorizationMiddleware>();
            app.UseMiddleware<AuditMiddleware>();

        }
    }
}
