using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Context;

namespace Backend.Core.Middleware
{
    public class SessionMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly TokenServiceI _tokenService;
        //private readonly SessionServiceI _sessionService;
        private readonly Serilog.ILogger _log;

        public SessionMiddleware(
            RequestDelegate next)
        {
            _next = next;
            //_tokenService = tokenService;
            //_sessionService = sessionService;
            _log = Log.Logger;
        }

        //public async Task InvokeAsync(
        //    HttpContext context,
        //    TokenServiceI _tokenService,
        //    SessionServiceI _sessionService)
        //{
        //    try
        //    {
        //        var authorizationHeader = context.Request.Headers.Authorization.ToString();

        //        if (!string.IsNullOrWhiteSpace(authorizationHeader) &&
        //            authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        //        {
        //            var token = authorizationHeader["Bearer ".Length..].Trim();

        //            var tv = _tokenService.DecodeToken(token);

        //            if (tv != null)
        //            {
        //                var session = _sessionService.GetSession(tv.SessionKey);

        //                if (session != null)
        //                {
        //                    // context.Items["session"] = session;

        //                    using (LogContext.PushProperty("SessionKey", tv.SessionKey))
        //                    {
        //                        await _next(context);
        //                        return;
        //                    }
        //                }
        //            }
        //        }

        //        await _next(context);
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.Error(ex, "Error loading session");

        //        throw;
        //    }
        //}
    }
}