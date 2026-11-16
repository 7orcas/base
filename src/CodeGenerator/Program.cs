using Microsoft.Extensions.Logging;

namespace CodeGenerator
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .SetMinimumLevel(LogLevel.Information)
                    .AddSimpleConsole();
                    //.AddSimpleConsole(options =>
                    //{
                    //    options.TimestampFormat = "HH:mm:ss ";
                    //});
            });

            ILogger logger = loggerFactory.CreateLogger<Program>();

            logger.LogInformation("Current Directory: {Directory}", Directory.GetCurrentDirectory());

            Console.WriteLine($"Current Directory: {Directory.GetCurrentDirectory()}");

            var template = new Templates.TrailTemplate().ConfigureTemplate();

            if (!template.IsValid())
            {
                Console.WriteLine($"Invalid Template: {template.ToEntity}");
            }

            var common = new CommonGenerator(template, logger);
            await common.Generate();

            var backend = new BackendGenerator(template, logger);
            await backend.Generate();
        }
    }
}
