using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;

namespace Backend.Core.Middleware
{
    public class AuditMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly Serilog.ILogger _log;

        public AuditMiddleware(
            RequestDelegate next)
        {
            _next = next;
            _log = Log.Logger;
        }

        public async Task InvokeAsync(
           HttpContext context,
           AuditServiceI _auditService)
        {

            try
            {
                var endpoint = context.GetEndpoint();
                var controllerActionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

                if (controllerActionDescriptor != null)
                {
                    var session = context.Items["session"] as SessionEnt;

                    //Method assign permission
                    MethodInfo methodInfo = controllerActionDescriptor.MethodInfo;

                    var audit = methodInfo.GetCustomAttribute<AuditListAtt>();
                    if (audit != null)
                        _auditService.ReadList(session, audit.EntityTypeId, null);
                    
                }

                await _next(context);
                return;
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error audit");
                throw;
            }
        }
    }
}
