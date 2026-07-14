using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;
using GC = Backend.GlobalConstants;

namespace Backend.Core.Middleware
{
    public class AuthorizationMiddleware
    {

        //Change to [AllowAnonymous]
        private readonly string[] nonAuthorisedMethods = {
            "LoginOptions", "Login", "GetToken", "LoginLabels", "LoginOrg",
            "RefreshExpiredToken", "RefreshCurrentToken",
            "ResetRequest", "ResetAction", "GetPasswordRules",
            "Signup", "VerifyEmail",
            "SetupMfa", "VerifyMfa"
        };

        private readonly RequestDelegate _next;
        private readonly Serilog.ILogger _log;

        public AuthorizationMiddleware(
            RequestDelegate next)
        {
            _next = next;
            _log = Log.Logger;
        }


        public async Task InvokeAsync(
           HttpContext context,
           PermissionServiceI _permissionService)
        {
            try
            {
                var endpoint = context.GetEndpoint();
                var controllerActionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

                if (controllerActionDescriptor != null)
                {
                    var authorised = false;
                    var session = context.Items["session"] as SessionEnt;
                    
                    //Method assign permission
                    MethodInfo methodInfo = controllerActionDescriptor.MethodInfo;

                    if (session == null)
                    {
                        authorised = nonAuthorisedMethods.Contains(methodInfo.Name);
                    }
                    else
                    {
                        var perm = methodInfo.GetCustomAttribute<PermissionAtt>();
                        var crud = methodInfo.GetCustomAttribute<CrudAtt>();

                        //Class assign permission
                        if (perm == null)
                        {
                            Type controllerType = controllerActionDescriptor.ControllerTypeInfo.AsType();
                            perm = controllerType.GetCustomAttribute<PermissionAtt>();
                        }
                        authorised = _permissionService.IsAuthorizedToAccessEndPoint(session, perm, crud);
                    }

                    if (authorised)
                    {
                        await _next(context);
                        return;
                    }

                    var key = session?.Key ?? "NoSession";
                    _log.Error("InterceptorNotAuthorised", context, key);
                    var r = new _ResponseDto
                    {
                        Valid = false,
                        ErrorMessage = "Not Authorised",  //ToDo label 'NAuth'
                        StatusCode = GC.StatusCodeNotAuthorised // HTTP status code
                    };
                    await context.Response.WriteAsJsonAsync(r); //TEST ME
                    return;
                }

                await _next(context);
                return;
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error authorisation");
                throw;
            }
        }


    }
}
