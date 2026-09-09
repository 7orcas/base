
using GC = Backend.GlobalConstants;

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
            SessionServiceI _sessionService,
            OrgServiceI _orgService,
        LabelServiceI _labelService)
        {
            var sessionKey = null as string;

            var authorizationHeader = context.Request.Headers.Authorization.ToString();

            //Get the session key from the authorization header
            if (!string.IsNullOrWhiteSpace(authorizationHeader) &&
                authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authorizationHeader["Bearer ".Length..].Trim();
                var tv = _tokenService.DecodeToken(token);
                if (tv != null)
                    sessionKey = tv.SessionKey;
            }

            //Get the session key for api testing (works in dev only)
            var apiTestKeyValue = context.Items[ApiDevMiddleware.DevSessionKey] as string;
            if (!string.IsNullOrEmpty(apiTestKeyValue))
                sessionKey = apiTestKeyValue;


            //Append the session if there is a session key
            if (sessionKey != null)
            {
                _diagnosticContext.Set("SessionKey", sessionKey);
                var session = _sessionService.GetSession(sessionKey);
                if (session != null)
                {
                    //Loaded here to aviod caching the labels in the session object
                    session.Org = await _orgService.GetOrg(session.OrgNr);

                    session.Labels = await _labelService.GetLangCodeDic(
                        session.UserConfig.LangCodeCurrent,
                        session.Org.LangLabelVariant);

                    context.Items["session"] = session;
                }
            }

            await _next(context);
        }

    }
}