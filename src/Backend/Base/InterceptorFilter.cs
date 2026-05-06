using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog.Events;
using System.Reflection;
using GC = Backend.GlobalConstants;

namespace Backend.Base
{
    /// <summary>
    /// Intercept all calls
    /// </summary>
    public class InterceptorFilter : ActionFilterAttribute
    {
        private readonly Serilog.ILogger _log;
        private readonly TokenServiceI _tokenService;
        private readonly SessionServiceI _sessionService;
        private readonly PermissionServiceI _permissionService;
        private readonly AuditServiceI _auditService;

        public InterceptorFilter(
            TokenServiceI tokenService,
            SessionServiceI sessionService,
            PermissionServiceI permissionService,
            AuditServiceI auditService)
        {
            _log = Serilog.Log.Logger;
            _tokenService = tokenService;
            _sessionService = sessionService;
            _permissionService = permissionService;
            _auditService = auditService;   
        }


        /// <summary>
        /// Intercept all calls and inspect for autorisation
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {   
            var sessionKey = null as string;
            var session = null as SessionEnt;

            // Extract the token and get session
            var authorizationHeader = context.HttpContext.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer"))
            {
                var token = authorizationHeader.Substring("Bearer".Length);
                var tv = _tokenService.DecodeToken(token);
                sessionKey = tv.SessionKey;
                session = _sessionService.GetSession(sessionKey);
                context.HttpContext.Items["session"] = session;
            }

            //Test permissions
            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                //Method assign permission
                MethodInfo methodInfo = controllerActionDescriptor.MethodInfo;
                var perm = methodInfo.GetCustomAttribute<PermissionAtt>();
                var crud = methodInfo.GetCustomAttribute<CrudAtt>();

                //Class assign permission
                if (perm == null)
                {
                    Type controllerType = controllerActionDescriptor.ControllerTypeInfo.AsType();
                    perm = controllerType.GetCustomAttribute<PermissionAtt>();
                }

                if (!_permissionService.IsAuthorizedToAccessEndPoint(session, perm, crud))
                {
                    Log(LogEventLevel.Error, "NotAuthorisedAccess call", context, sessionKey);
                    var r = new _ResponseDto
                    {
                        Valid = false,
                        ErrorMessage = "Not Authorised",  //ToDo label 'NAuth'
                        StatusCode = GC.StatusCodeNotAuthorised // HTTP status code
                    };
                    context.Result = new OkObjectResult(r);
                }

                //Audit list action
                var audit = methodInfo.GetCustomAttribute<AuditListAtt>();
                if (audit != null)
                    _auditService.ReadList(session, audit.EntityTypeId, null);
                


            }

            if (_log.IsEnabled(LogEventLevel.Debug))
                Log(LogEventLevel.Debug, "Interceptor call", context, sessionKey);

        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // Intercept after the controller action executes
            
            if (context.Exception != null)
            {
                var r = new _ResponseDto
                {
                    Valid = false,
                    ErrorMessage = context.Exception.Message,
                    StatusCode = 500 
                };
                context.Result = new OkObjectResult(r);
            }

        }

        private void Log(LogEventLevel level, string message, ActionExecutingContext context, string? sessionKey)
        {
            var logstring = "";
            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptorX)
            {
                logstring = controllerActionDescriptorX.ControllerName;

                MethodInfo methodInfo = controllerActionDescriptorX.MethodInfo;
                logstring += "." + methodInfo.Name;

                var p = methodInfo.GetCustomAttribute<PermissionAtt>();
                if (p != null)
                    logstring += ", perm: " + p.Nr;

                var c = methodInfo.GetCustomAttribute<CrudAtt>();
                if (c != null)
                    logstring += ", crud: " + c.Action;

                if (sessionKey != null)
                    logstring += ", SessionKey: " + sessionKey;

                logstring = message + ": " + logstring;

                if (level == LogEventLevel.Debug) 
                    _log.Debug(logstring);

                if (level == LogEventLevel.Error)
                    _log.Error(logstring);
            }
        }

    }
}

