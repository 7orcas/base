
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
            OrgServiceI _orgService,
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
                    /*
                      The request header value is a concatation of:
                      - GC.ApiTestKeyValue + the current date - 1 year
                      - user name
                      - password
                      - org nr
                     */

                    try
                    {
                        string[] values = apiKey.Split(',');

                        var datePart = values[0].Substring(GC.ApiTestKeyValue.Length, 6);
                        var username = values[1];
                        var password = values[2];
                        var orgNr = int.Parse(values[3]);

                        var date = DateTime.ParseExact(datePart,  "yyMMdd",  null);
                        if (date.Date != DateTime.Now.AddYears(-1).Date)
                            throw new Exception();

                        var org = await _orgService.GetOrg(orgNr);

                        var request = new LoginRequest
                        {
                            UserName = username,
                            Password = password,
                            Org = orgNr,
                            LangCode = "en",
                            SourceApplication = GC.ApiClient,
                            ApiKey = org.ApiKey,
                            ApiTestKeyValue = apiKey
                        };
                        var login = await _loginService.LoginUser("local", request, true);

                        if (!login.Response.IsValid)
                            throw new Exception();

                        session = _sessionService.GetSession(apiKey);
                        session.BearerToken = login.Response.TokenKey;
                    }
                    catch 
                    {
                        _log.Warning("Failed to login user for API test key: {ApiKey}", apiKey);
                        context.Response.StatusCode = 401;
                        return;
                    }
                }

                context.Items[DevSessionKey] = apiKey;
                context.Request.Headers["Authorization"] = $"Bearer {session.BearerToken}";
            }

            await _next(context);
        }

    }
}