using DiffMatchPatch;
using DocumentFormat.OpenXml.Office2010.Excel;
using JsonDiffPatchDotNet;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
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
            var audit = null as AuditListAtt;

            try
            {
                var endpoint = context.GetEndpoint();
                var controllerActionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

                audit = GetAuditListAtt(controllerActionDescriptor);

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

            //No action
            if (audit == null 
                || audit.EntityTypeNr == -1 
                || audit.CrudAction == null) 
                return;

            //Invalid response
            if (context.Items[AuditCapture] != "true")
                return;

            try
            {

                if (audit.CrudAction == GC.CrudReadList)
                {
                    LogReads(context, _auditService, session, audit.EntityTypeNr, audit.CrudAction);
                    return;
                }

                if (audit.CrudAction == GC.CrudRead)
                {
                    LogReads(context, _auditService, session, audit.EntityTypeNr, audit.CrudAction);
                    return;
                }

                if (audit.CrudAction == GC.CrudUpdate)
                {
                    LogUpdates(context, _auditService, session, audit.EntityTypeNr);
                    return;
                }
                
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error AuditMiddleware (2)");
            }
        }

        private void LogReads(HttpContext context, AuditServiceI _auditService, SessionEnt session, int entityTypeNr, string crudAction)
        {
            long? id = null;
            int? version = GetEntityVersion(context);
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

            _auditService.LogAction(session, entityTypeNr, id, version, crudAction, details);
        }

        private void LogUpdates(HttpContext context, AuditServiceI _auditService, SessionEnt session, int entityTypeNr)
        {
            LogUpdate(context, _auditService, session, entityTypeNr, GC.CrudCreate, AuditCreate);
            LogUpdate(context, _auditService, session, entityTypeNr, GC.CrudDelete, AuditDelete);
            LogUpdate(context, _auditService, session, entityTypeNr, GC.CrudUpdate, AuditUpdate);
        }

        private void LogUpdate(HttpContext context, AuditServiceI _auditService, SessionEnt session, int entityTypeNr, string crud, string param)
        {
            if (context.Items.TryGetValue(param, out var records))
            {
                var dtos = records as Dictionary<long, AuditInfo>;
                foreach (var kvp in dtos)
                    _auditService.LogAction(session, entityTypeNr, kvp.Key, kvp.Value.Version, crud, kvp.Value.Json);
            }
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

        private int? GetEntityVersion(HttpContext context)
        {
            if (context.Items.TryGetValue(AuditInfo, out var record))
            {
                var info = record as AuditInfo;
                return info.Version;
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
