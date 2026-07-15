using System.Net;
using GC = Backend.GlobalConstants;

namespace Backend.Core.Middleware
{
    public class ExceptionMiddleware : BaseMiddleware
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

        public async Task InvokeAsync(
            HttpContext context,
            LabelServiceI _labelService)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var id = Guid.NewGuid().ToString();

                _log.Error(ex, "Unhandled exception while processing {Method} {Path} Error Id:{id}",
                    context.Request.Method,
                    context.Request.Path,
                    id);

                await HandleExceptionAsync(context, ex, id, _labelService);
            }
        }

        private async Task HandleExceptionAsync(
            HttpContext context,
            Exception ex,
            string id,
            LabelServiceI _labelService)
        {
            var labels = await _labelService.GetLangCodeDic(GC.LangCodeDefault, GC.LangLabelVariantDefault);

            var r = new _ResponseDto
            {
                Valid = false,
                StatusCode = (int)HttpStatusCode.InternalServerError
            };


            if (!_environment.IsDevelopment())
            {
                r.ErrorMessage = "[" + id + "]  " + ex.ToString();
                context.Response.ContentType = "application/problem+json";
            }
            else
            {
                r.ErrorMessage = GetLabel("UnKnError", "An unexpected error occurred", labels) + " (id:" + id + ")";
            }

            await context.Response.WriteAsJsonAsync(r);
        }
    }
}

