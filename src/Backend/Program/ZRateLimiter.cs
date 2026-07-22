using System.Net;
using System.Threading.RateLimiting;

namespace Backend.Program
{
    public class ZRateLimiter
    {

        static public void Configure(WebApplicationBuilder builder)
        {
            var dft = new RateLimiting
            {
                PermitLimit = int.Parse(builder.Configuration["RateLimiting:Default:PermitLimit"]),
                WindowMinutes = int.Parse(builder.Configuration["RateLimiting:Default:WindowMinutes"])
            };

            var login = new RateLimiting
            {
                PermitLimit = int.Parse(builder.Configuration["RateLimiting:Login:PermitLimit"]),
                WindowMinutes = int.Parse(builder.Configuration["RateLimiting:Login:WindowMinutes"])
            };

            var high = new RateLimiting
            {
                PermitLimit = int.Parse(builder.Configuration["RateLimiting:HighVolume:PermitLimit"]),
                WindowMinutes = int.Parse(builder.Configuration["RateLimiting:HighVolume:WindowMinutes"])
            };


            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.OnRejected = async (context, token) =>
                {
                    var httpContext = context.HttpContext;

                    Log.Warning(
                        "Rate limit exceeded. Path={Path}, IP={IP}",
                        httpContext.Request.Path,
                        httpContext.Connection.RemoteIpAddress);

                    httpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    httpContext.Response.ContentType = "application/json";

                    var r = new _ResponseDto
                    {
                        Valid = false,
                        StatusCode = (int)HttpStatusCode.TooManyRequests,
                        ErrorMessage = "Too many requests, please try again later."
                    };
                    await httpContext.Response.WriteAsJsonAsync(r);

                    //await Task.CompletedTask;
                };

                // DEFAULT POLICY
                options.GlobalLimiter =
                    PartitionedRateLimiter.Create<HttpContext, string>(context =>
                        RateLimitPartition.GetFixedWindowLimiter(
                            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                            _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = dft.PermitLimit,
                                Window = TimeSpan.FromHours(dft.WindowMinutes),
                                QueueLimit = 0
                            }));

                // LOGIN POLICY
                options.AddPolicy("login", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = login.PermitLimit,
                            Window = TimeSpan.FromMinutes(login.WindowMinutes),
                            QueueLimit = 0
                        }));

                // HIGH VOLUME POLICY
                options.AddPolicy("highvolume", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = high.PermitLimit,
                            Window = TimeSpan.FromHours(high.WindowMinutes),
                            QueueLimit = 0
                        }));
            });
        }

        static public void Use(WebApplication app)
        {
            app.UseRateLimiter();
        }

    }
}
