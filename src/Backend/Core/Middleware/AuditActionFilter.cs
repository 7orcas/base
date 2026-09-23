using DiffMatchPatch;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using JsonDiffPatchDotNet;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Runtime.Intrinsics.Arm;
using System.Text.Json;
using System.Text.Json.Nodes;
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

                var info = new AuditInfo();
                http.Items[AuditInfo] = info;

                //Get call parameters (if exist)
                if (http.Items.TryGetValue(AuditArg, out var value))
                {
                    var args = value as Dictionary<string, object?>;

                    foreach (var arg in args)
                    {
                        if (info.Id == null) info.Id = GetEntityId(arg);
                        if (info.Json == null) info.Json = GetSearch(arg, response.RecordCount);
                    }
                }

                //Individual objects (ie getById)
                var dto = response.Result as _BaseDto;
                if (dto != null)
                {
                    info.Code = dto.Code;
                    info.Version = dto.Version;
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

        private string? GetSearch(KeyValuePair<string, object?> arg, int? recordCount)
        {
            var rc = recordCount.HasValue ? recordCount.ToString() : "";
            if (arg.Value is _BaseSearch search)
            {
                var json = JsonSerializer.Serialize(search, search.GetType(), options);
                var obj = JsonNode.Parse(json)?.AsObject();
                if (obj != null)
                {
                    obj["RecordsReturned"] = rc;
                    return obj.ToJsonString(options);
                }
            }
            return null;
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


                if (dto.IsDelete)
                {
                    var node = JsonNode.Parse(
                        JsonSerializer.Serialize(dto, dto.GetType(), options)
                    );
                    RemoveObjectsWithZeroId(node);

                    deletes[dto.Id] = new AuditInfo{
                        Id = dto.Id,
                        Code = dto.Code,
                        Version = dto.Version,
                        Json = node.ToString()
                    };
                    continue;
                }

                var update = listUpdate.FirstOrDefault(x => x.Id == dto.Id);
                if (update == null)
                    continue;


                if (dto.Version <= 0)
                {
                    var node = JsonNode.Parse(
                        JsonSerializer.Serialize(update, update.GetType(), options)
                    );
                    RemoveObjectsWithZeroId(node);

                    creates[dto.Id] = new AuditInfo
                    {
                        Id = update.Id,
                        Code = update.Code,
                        Version = update.Version,
                        Json = node.ToString()
                    };
                }
                else
                {
                    var before = JsonSerializer.Serialize(dto, dto.GetType());
                    var after = JsonSerializer.Serialize(update, update.GetType());
                    var diffJson = jdp.Diff(before, after);
                    var diffFormat = "";
                    if (!string.IsNullOrWhiteSpace(diffJson))
                    {
                        var diff = JToken.Parse(diffJson);
                        diffFormat = AuditJsonDiff.ToAuditEntries(diff);
                    }

                    updates[dto.Id] = new AuditInfo
                    {
                        Id = update.Id,
                        Code = update.Code,
                        Version = dto.Version,
                        Json = diffFormat
                    };
                }
            }

            context.Items[AuditCreate] = creates;
            context.Items[AuditUpdate] = updates;
            context.Items[AuditDelete] = deletes;
        }

        private static void RemoveObjectsWithZeroId(JsonNode? node)
        {
            if (node is JsonArray array)
            {
                for (int i = array.Count - 1; i >= 0; i--)
                {
                    var item = array[i];

                    if (item is JsonObject obj &&
                        obj.TryGetPropertyValue("Id", out var idNode) &&
                        idNode is not null &&
                        int.TryParse(idNode.ToJsonString(), out var id) &&
                        id <= 0)
                    {
                        array.RemoveAt(i);
                        continue;
                    }

                    RemoveObjectsWithZeroId(item);
                }
            }
            else if (node is JsonObject obj)
            {
                foreach (var property in obj.ToList())
                {
                    RemoveObjectsWithZeroId(property.Value);
                }
            }
        }

        private JsonSerializerOptions options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

    }


}
