using System.Net;

namespace Backend.Core.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Serilog.ILogger _log;
        private readonly IHostEnvironment _environment;

        public ExceptionMiddleware(
            RequestDelegate next,
            IHostEnvironment environment)
        {
            _next = next;
            _environment = environment;
            _log = Log.Logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _log.Error("Unhandled exception while processing {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(
            HttpContext context,
            Exception ex)
        {
            var r = new _ResponseDto
            {
                Valid = false,
                StatusCode = (int)HttpStatusCode.InternalServerError
            };


            if (_environment.IsDevelopment())
            {
                r.ErrorMessage = ex.ToString();
                context.Response.ContentType = "application/problem+json";
            }
            else
            {
                r.ErrorMessage = "An unexpected error occurred";
            }

            await context.Response.WriteAsJsonAsync(r);
        }
    }
}

