
namespace Backend.Core.Middleware
{
    public class SessionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Serilog.ILogger _log;
        
        public SessionMiddleware(
            RequestDelegate next)
        {
            _next = next;
            _log = Log.Logger;
        }

        public async Task InvokeAsync(
            HttpContext context,
            Serilog.IDiagnosticContext _diagnosticContext,
            TokenServiceI _tokenService,
            SessionServiceI _sessionService)
        {
            var authorizationHeader = context.Request.Headers.Authorization.ToString();

            if (!string.IsNullOrWhiteSpace(authorizationHeader) &&
                authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authorizationHeader["Bearer ".Length..].Trim();

                var tv = _tokenService.DecodeToken(token);
                if (tv != null)
                {
                    _diagnosticContext.Set("SessionKey", tv.SessionKey);
                    var session = _sessionService.GetSession(tv.SessionKey);
                    if (session != null)
                        context.Items["session"] = session;
                }
            }
            await _next(context);
        }

    }
}