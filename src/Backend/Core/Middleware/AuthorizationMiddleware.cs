using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Org.BouncyCastle.Security;
using System.Reflection;
using GC = Backend.GlobalConstants;

namespace Backend.Core.Middleware
{
    public class AuthorizationMiddleware : BaseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Serilog.ILogger _log;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
            _log = Log.Logger;
        }


        public async Task InvokeAsync(
           HttpContext context,
           LabelServiceI _labelService,
           PermissionServiceI _permissionService)
        {
            var endpoint = context.GetEndpoint();
            var allowAnonymous = endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null;

            if (allowAnonymous)
            {
                await _next(context);
                return;
            }

            var controllerActionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
            if (controllerActionDescriptor == null 
                && context.Request.Path == "/api/Login/login")  //FIX ME I don't know why/how this call is being made
            {
                await _next(context);
                return;
            }
            else if (controllerActionDescriptor == null)
            {
                var rx = await HandleException(null, context, _labelService);
                await context.Response.WriteAsJsonAsync(rx);
                return;
            }


            var session = context.Items["session"] as SessionEnt;
            if (session != null)
            {
                //Method assigns permissions
                MethodInfo methodInfo = controllerActionDescriptor.MethodInfo;
                var perm = methodInfo.GetCustomAttribute<PermissionAtt>();
                var crud = methodInfo.GetCustomAttribute<CrudAtt>();

                //Class assign permission
                if (perm == null)
                {
                    Type controllerType = controllerActionDescriptor.ControllerTypeInfo.AsType();
                    perm = controllerType.GetCustomAttribute<PermissionAtt>();
                }
                    
                if (_permissionService.IsAuthorizedToAccessEndPoint(session, perm, crud))
                {
                    await _next(context);
                    return;
                }
            }

            var r = await HandleException(session, context, _labelService);
            await context.Response.WriteAsJsonAsync(r);
        }

        private async Task<_ResponseDto> HandleException(SessionEnt session, HttpContext context, LabelServiceI _labelService)
        {
            var labels = await _labelService.GetLangCodeDic(GC.LangCodeDefault, GC.LangLabelVariantDefault);
            
            var key = session?.Key ?? "NoSession";
            _log.Error("Not Authorised, path {Path} session {key} ", context.Request.Path, key);

            var r = new _ResponseDto
            {
                Valid = false,
                ErrorMessage = GetLabel("NAuthX", "Not Authorised", labels),
                StatusCode = GC.StatusCodeNotAuthorised // HTTP status code
            };

            return r;
        }

    }
}
