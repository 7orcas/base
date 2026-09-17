using DiffMatchPatch;
using DocumentFormat.OpenXml.Office2010.Excel;
using JsonDiffPatchDotNet;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Net.WebRequestMethods;
using GC = Backend.GlobalConstants;

namespace Backend.Core.Middleware
{
    public class AuditMiddleware : AuditActionConstants
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
            int entityTypeNr = -1;
            string crudAction = null;

            try
            {
                var endpoint = context.GetEndpoint();
                var controllerActionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

                var audit = GetAuditListAtt(controllerActionDescriptor);

                if (audit != null)
                {
                    session = context.Items["session"] as SessionEnt;
                    context.Items[AuditCapture] = "true";
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error AuditMiddleware");
                return;
            }

            //Continue
            await _next(context);

            //Invalid response
            if (context.Items[AuditCapture] != null &&
                context.Items[AuditCapture] != "true")
                return;

            try
            {
                if (entityTypeNr == -1 || crudAction == null) return;

                if (crudAction == GC.CrudRead || crudAction == GC.CrudReadList)
                {
                    LogReads(context, _auditService, session, entityTypeNr, crudAction);
                    return;
                }

                if (crudAction == GC.CrudUpdate)
                {
                    LogUpdates(context, _auditService, session, entityTypeNr, 0L, crudAction);
                    return;
                }

                if (crudAction == GC.CrudDelete)
                {
                    LogDeletes(context, _auditService, session, entityTypeNr, 0L, crudAction);
                    return;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error AuditMiddleware (2)");
                return;
            }
        }

        private void LogReads(HttpContext context, AuditServiceI _auditService, SessionEnt session, int entityTypeNr, string crudAction)
        {
            long? id = null;
            string? details = null;

            //Get passed in parameters (captured in AuditActionFilter)
            if (context.Items.TryGetValue(AuditArg, out var value))
            {
                var args = value as Dictionary<string, object?>;

                foreach (var arg in args)
                {
                    if (id == null) id = GetEntityId(arg);
                    if (details == null) details = GetSearch(arg);
                }
            }

            _auditService.LogAction(session, entityTypeNr, id, crudAction, details);
        }

        private void LogUpdates(HttpContext context, AuditServiceI _auditService, SessionEnt session, int entityTypeNr, long entityId, string crudAction)
        {
            var jdp = new JsonDiffPatch();


            if (context.Items.TryGetValue(AuditBefore + AuditC, out var creates))
            {
                var dtos = creates as Dictionary<long, string>;
                foreach (var kvp in dtos)
                    _auditService.LogAction(session, entityTypeNr, kvp.Key, GC.CrudCreate, kvp.Value);
            }

            if (context.Items.TryGetValue(AuditBefore + AuditD, out var deletes))
            {
                var dtos = deletes as Dictionary<long, string>;
                foreach (var kvp in dtos)
                    _auditService.LogAction(session, entityTypeNr, kvp.Key, GC.CrudDelete, kvp.Value);
            }

            if (context.Items.TryGetValue(AuditBefore + AuditU, out var updates))
            {
                var before = updates as Dictionary<long, string>;
                var after = new Dictionary<long, string>();

                if (context.Items.TryGetValue(AuditAfter + AuditU, out var ss))
                    after = ss as Dictionary<long, string>;
                
                foreach (var kvp in before)
                {
                    var diff = "Not configured";
                    if (after.ContainsKey(kvp.Key))
                        diff = jdp.Diff(kvp.Value, after[kvp.Key]);
                    _auditService.LogAction(session, entityTypeNr, kvp.Key, GC.CrudUpdate, diff);
                }
            }
        }



        private void LogDeletes(HttpContext context, AuditServiceI _auditService, SessionEnt session, int entityTypeNr, long entityId, string crudAction)
        {

            var jdp = new JsonDiffPatch();

            //JToken left = JToken.Parse(oldJson);
            //JToken right = JToken.Parse(newJson);

            //var diff = jdp.Diff(left, right);

            _auditService.LogAction(session, entityTypeNr, entityId, crudAction, "delete");
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
