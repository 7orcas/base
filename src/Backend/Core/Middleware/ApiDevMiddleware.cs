
using GC = Backend.GlobalConstants;

namespace Backend.Core.Middleware
{
    public class ApiDevMiddleware
    {
        public const string DevSessionKey = "dev-session-key";
        private readonly Serilog.ILogger _log;
        private readonly RequestDelegate _next;
        
        public ApiDevMiddleware(
            RequestDelegate next)
        {
            _next = next;
            _log = Log.Logger;
        }

        public async Task InvokeAsync(
            HttpContext context,
            LoginServiceI _loginService,
            SessionServiceI _sessionService)
        {
            var apiKey = context.Request.Headers[GC.ApiTestKey].FirstOrDefault();

            if (!string.IsNullOrEmpty(apiKey) 
                && apiKey.StartsWith(GC.ApiTestKeyValue))
            {
                var session = _sessionService.GetSession(apiKey);

                //No session, then login
                if (session == null)
                {
                    var request = new LoginRequest
                    {
                        UserName = AppSettings.ServiceAccount.Username,
                        Password = apiKey.Substring(GC.ApiTestKeyValue.Length),
                        Org = 1,
                        LangCode = "en",
                        SourceApplication = GC.ApiClient,
                        ApiKey = "X123",
                        ApiTestKeyValue = apiKey
                    };
                    var login = await _loginService.LoginUser("local", request, true);

                    if (!login.Response.IsValid)
                    {
                        _log.Warning("Failed to login user for API test key: {ApiKey}", apiKey);
                        context.Response.StatusCode = 401;
                        return;
                    }
                    session = _sessionService.GetSession(apiKey);
                    session.BearerToken = login.Response.TokenKey;
                }

                context.Items[DevSessionKey] = apiKey;
                context.Request.Headers["Authorization"] = $"Bearer {session.BearerToken}";
            }

            await _next(context);
        }

    }
}