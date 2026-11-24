using Backend.Base.Label.Ent;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Security;

namespace CodeGenerator
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var projectDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File(
                    Path.Combine(projectDir, "Logs", "log-.txt"),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Message:lj}{NewLine}")
                .CreateLogger();

            try
            {
                using var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddSerilog();
                });

                Microsoft.Extensions.Logging.ILogger logger = loggerFactory.CreateLogger<Program>();

                logger.LogInformation("Application started: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                
                var template = new Templates.TrialGroupTemplate().ConfigureTemplate();

                if (!template.IsValid())
                {
                    Console.WriteLine($"Invalid Template: {template.ToEntity}");
                    logger.LogInformation($"Invalid Template: {template.ToEntity}");
                    return;
                }

                logger.LogInformation($"Application = {template.Application}");
                logger.LogInformation($"LangKey = {template.LangKey}");
                logger.LogInformation($"FromEntity = {template.FromEntity}");
                logger.LogInformation($"FromPermission = {template.FromPermission}");
                logger.LogInformation($"ToEntity = {template.ToEntity}");
                logger.LogInformation($"ToPermission = {template.ToPermission}");
                logger.LogInformation($"ToPermissionValue = {template.ToPermissionValue}");
                logger.LogInformation($"EntityTypeValue = {template.EntityTypeValue}");
                logger.LogInformation($"Route = {template.Route}");
                logger.LogInformation($"PageCode = {template.PageCode}");
                logger.LogInformation($"MenuItem = {template.MenuItem}");
                logger.LogInformation($"MenuItemLangKey = {template.MenuItemLangKey}");


                logger.LogInformation(Environment.NewLine + ">>>CommonGenerator");
                var common = new CommonGenerator(template, logger);
                await common.Generate();

                logger.LogInformation(Environment.NewLine + ">>>BackendGenerator");
                var backend = new BackendGenerator(template, logger);
                await backend.Generate();

                logger.LogInformation(Environment.NewLine + ">>>FrontendGenerator");
                var frontend = new FrontendGenerator(template, logger);
                await frontend.Generate();
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
