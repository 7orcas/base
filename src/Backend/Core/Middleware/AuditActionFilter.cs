using DiffMatchPatch;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using JsonDiffPatchDotNet;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Runtime.Intrinsics.Arm;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backend.Core.Middleware
{
    public class AuditActionFilter : AuditActionConstants, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context) 
        {
            //Get request arguments
            var http = context.HttpContext;
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
                    response.AuditObject = null;
                    http.Items[AuditCapture] = "false";
                    return;
                }
                
                //Individual objects (ie getById)
                var dto = response.Result as _BaseDto;
                if (dto != null)
                {
                    CaptureGetById(http, dto);
                    return;
                }


                //Updates
                var listBefore = response.AuditObject as IEnumerable<_BaseDto>;
                response.AuditObject = null;
                var listUpdate = response.Result as IEnumerable<_BaseDto>;
                if (listBefore != null && listUpdate != null)
                    CaptureUpdates(http, listBefore, listUpdate);
            }
        }

        private void CaptureGetById(HttpContext context, _BaseDto dto)
        {
            context.Items[AuditInfo] = new AuditInfo
            {
                Version = dto.Version
            };
        }


        private void CaptureUpdates(HttpContext context,
            IEnumerable<_BaseDto> listBefore,
            IEnumerable<_BaseDto> listUpdate)
        {
            var jdp = new JsonDiffPatch();
            var creates = new Dictionary<long, AuditInfo>();
            var updates = new Dictionary<long, AuditInfo>();
            var deletes = new Dictionary<long, AuditInfo>();

            foreach (var dto in listBefore)
            {
                if (dto.IsDelete && dto.IsNew())
                    continue; //Ignore

                var before = JsonSerializer.Serialize(dto, dto.GetType());

                if (dto.IsDelete)
                {
                    deletes[dto.Id] = new AuditInfo{
                        Json = before,
                        Version = dto.Version
                    };
                    continue;
                }

                var update = listUpdate.FirstOrDefault(x => x.Id == dto.Id);
                var after = JsonSerializer.Serialize(update, update.GetType());

                if (dto.Version <= 0)
                    creates[dto.Id] = new AuditInfo
                    {
                        Json = after,
                        Version = dto.Version
                    };
                else
                {
                    var diff = jdp.Diff(before, after);
                    updates[dto.Id] = new AuditInfo
                    {
                        Json = diff,
                        Version = dto.Version
                    };
                }
            }

            context.Items[AuditCreate] = creates;
            context.Items[AuditUpdate] = updates;
            context.Items[AuditDelete] = deletes;
        }

    }
}
