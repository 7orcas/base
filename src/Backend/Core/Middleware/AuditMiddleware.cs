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
                || audit.Action == null) 
                return;

            //Invalid response
            if (context.Items[AuditCapture] != "true")
                return;

            try
            {

                if (audit.Action == GC.CrudReadList 
                    || audit.Action == GC.CrudRead)
                {
                    LogReads(session, context, _auditService, audit);
                    return;
                }


                if (audit.Action == GC.CrudUpdate)
                {
                    LogUpdates(session, context, _auditService, audit);
                    return;
                }
                
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error AuditMiddleware (2)");
            }
        }

        private void LogReads(SessionEnt session, HttpContext context, AuditServiceI _auditService, AuditListAtt attr)
        {
            AuditInfo info = GetAuditInfo(context);
            _auditService.LogAction(session, attr.EntityTypeNr, attr.Action, info);
        }

        private void LogUpdates(SessionEnt session, HttpContext context, AuditServiceI _auditService, AuditListAtt attr)
        {
            LogUpdate(session, context, _auditService,attr.EntityTypeNr, GC.CrudCreate, AuditCreate);
            LogUpdate(session, context, _auditService,attr.EntityTypeNr, GC.CrudDelete, AuditDelete);
            LogUpdate(session, context, _auditService,attr.EntityTypeNr, GC.CrudUpdate, AuditUpdate);
        }

        private void LogUpdate(SessionEnt session, HttpContext context, AuditServiceI _auditService, int entityTypeNr, string crud, string param)
        {
            if (context.Items.TryGetValue(param, out var records))
            {
                var dtos = records as Dictionary<long, AuditInfo>;
                foreach (var kvp in dtos)
                    _auditService.LogAction(session, entityTypeNr, crud, kvp.Value);
            }
        }

        

        private AuditInfo GetAuditInfo(HttpContext context)
        {
            if (context.Items.TryGetValue(AuditInfo, out var record))
            {
                var info = record as AuditInfo;
                return info;
            }
            return new AuditInfo();
        }

        


    }
}
