using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backend.Core.Middleware
{
    public class AuditActionFilter : AuditActionConstants, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var http = context.HttpContext;

            //1. Is this an update request
            var updateRequest = context.ActionArguments.Values
                    .OfType<UpdateRequestI>()
                    .FirstOrDefault();

            if (updateRequest != null)
            {
                CaptureUpdates(AuditBefore, http, updateRequest.Updates);
                return;
            }

            //2. Assume a read request
            http.Items[AuditArg] = new Dictionary<string, object?>(context.ActionArguments);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var http = context.HttpContext;

            if (http.Items[AuditCapture] == null ||
                http.Items[AuditCapture] != "true")
                return;
            
            if (context.Result is OkObjectResult okResult &&
                okResult.Value is _ResponseDto response)
            {
                if (!response.Valid)
                {
                    http.Items[AuditCapture] = "false";
                    return;
                }
                    
                var list = response.Result as IEnumerable<_BaseDto>;
                if (list != null)
                    CaptureUpdates(AuditAfter, http, list);
            }
        }

        private void CaptureUpdates(string prefix,
            HttpContext context,
            IEnumerable<_BaseDto> list)
        {
            var creates = new Dictionary<long, string>();
            var updates = new Dictionary<long, string>();
            var deletes = new Dictionary<long, string>();

            foreach (var dto in list)
            {
                if (dto.IsDelete && dto.IsNew())
                    continue; //Ignore

                var ss = JsonSerializer.Serialize(dto, dto.GetType());

                if (dto.IsDeleteable())
                    deletes[dto.Id] = ss;
                else if (dto.IsNewable())
                    creates[dto.Id] = ss;
                else
                    updates[dto.Id] = ss;
            }

            context.Items[prefix + AuditC] = creates;
            context.Items[prefix + AuditU] = updates;
            context.Items[prefix + AuditD] = deletes;
        }

    }
}
