using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Text.Json;
using JsonDiffPatchDotNet;
using System.Text.Json.Serialization;
using GC = Backend.GlobalConstants;

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

            var session = null as SessionEnt;
            int entityTypeId = -1;
            string crudAction = null;

            try
            {
                var endpoint = context.GetEndpoint();
                var controllerActionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

                if (controllerActionDescriptor != null)
                {
                    session = context.Items["session"] as SessionEnt;

                    //Method ignore
                    MethodInfo methodInfo = controllerActionDescriptor.MethodInfo;
                    var ignore = methodInfo.GetCustomAttribute<AuditIgnoreAtt>();

                    //If not ignore then get entity type
                    if (ignore == null)
                    {
                        //Method assign attibutes (first priority)
                        var audit = methodInfo.GetCustomAttribute<AuditListAtt>();
                        if (audit != null)
                        {
                            entityTypeId = audit.EntityTypeId;
                            crudAction = audit.CrudAction;
                        }

                        //Class assigned attibutes (second priority)
                        var controllerType = controllerActionDescriptor.ControllerTypeInfo;
                        var classAudit = controllerType.GetCustomAttribute<AuditListAtt>();
                        if (classAudit != null)
                        {
                            if (entityTypeId == -1)
                                entityTypeId = classAudit.EntityTypeId;
                            if (crudAction == null)
                                crudAction = classAudit.CrudAction;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error AuditMiddleware");
                return;
            }

            //Continue
            await _next(context);

            if (entityTypeId == -1 || crudAction == null) return;

            if (crudAction == GC.CrudRead || crudAction == GC.CrudReadList)
            {
                LogReads(context, _auditService, session, entityTypeId, crudAction);
                return;
            }




        }

        private void LogReads(HttpContext context, AuditServiceI _auditService, SessionEnt session, int entityTypeId, string crudAction)
        {
            long? id = null;
            string? details = null;

            //Get passed in parameters (captured in AuditActionFilter)
            if (context.Items.TryGetValue("ActionArguments", out var value))
            {
                var args = value as Dictionary<string, object?>;

                foreach (var arg in args)
                {
                    if (id == null) id = GetEntityId(arg);
                    if (details == null) details = GetSearch(arg);
                }
            }

            _auditService.LogAction(session, entityTypeId, id, crudAction, details);
        }

        private void LogUpdates(HttpContext context, AuditServiceI _auditService, SessionEnt session, int entityTypeId, long entityId, string crudAction)
        {

            var jdp = new JsonDiffPatch();

            //JToken left = JToken.Parse(oldJson);
            //JToken right = JToken.Parse(newJson);

            //var diff = jdp.Diff(left, right);

            //_auditService.LogAction(session, entityTypeId, entityId, crudAction, diff.ToString());
        }


        private long? GetEntityId(KeyValuePair<string, object?> arg)
        {
            if (arg.Key.ToLower() == "id" && arg.Value != null)
            {
                if (long.TryParse(arg.Value.ToString(), out long parsedId))
                {
                    return parsedId;
                }
            }
            return null;
        }

        private string? GetSearch(KeyValuePair<string, object?> arg)
        {
            if (arg.Value is _BaseSearch search)
            {
                return JsonSerializer.Serialize(search,
                    search.GetType(),
                    new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    }
                    );
            }
            return null;
        }


    }
}
