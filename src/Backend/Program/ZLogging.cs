namespace Backend.Program
{
    public class ZLogging
    {
        static public void Configure(WebApplicationBuilder builder)
        {
            // Add Serilog configuration
            // Set up Serilog to use appsettings.json
            builder.Logging.ClearProviders();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration) // Reads from appsettings.json
                .Enrich.FromLogContext() //wrap log entries with additional context info (e.g. request details)
                .CreateLogger();

            builder.Host.UseSerilog();

        }

        static public void ConfigureRequests(WebApplication app)
        {
            app.UseSerilogRequestLogging(options =>
            {
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestPath", httpContext.Request.Path);
                    diagnosticContext.Set("RequestMethod", httpContext.Request.Method);
                };
            });
        }

    }
}
