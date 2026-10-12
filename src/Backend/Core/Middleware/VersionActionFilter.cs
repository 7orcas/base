using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using GC = Backend.GlobalConstants;

namespace Backend.Core.Middleware
{
    public class VersionActionFilter : IAsyncActionFilter
    {
        private readonly Serilog.ILogger _log;

        public VersionActionFilter()
        {
            _log = Log.Logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var versionAtt = context.ActionDescriptor.EndpointMetadata
                            .OfType<VersionAtt>()
                            .FirstOrDefault();

            if (versionAtt != null)
            {
                var http = context.HttpContext;
                var session = http.Items["session"] as SessionEnt;

                if (http.Items.TryGetValue(GC.RestActionArg, out var value))
                {
                    var args = value as Dictionary<string, object?>;
                    var vals = new List<ValidationDto>();
                    ValidationDto m = null;

                    foreach (var arg in args)
                    {
                        switch (arg.Value)
                        {
                            case UpdateRequestI req:
                                if (session.IsService && req.OverrideWarning) break; //Skip version check 
                                foreach (var dto in req.Updates)
                                {
                                    m = await ValidateVersion(session, dto, versionAtt);
                                    if (m != null) vals.Add(m);
                                }
                                break;

                            case _BaseDto dto:
                                m = await ValidateVersion(session, dto, versionAtt);
                                if (m != null) vals.Add(m);
                                break;

                            case IEnumerable<_BaseDto> dtos:
                                foreach (var dto in dtos)
                                {
                                    m = await ValidateVersion(session, dto, versionAtt);
                                    if (m != null) vals.Add(m);
                                }
                                break;
                        }
                    }

                    if (vals.Count > 0)
                    {
                        context.Result = new OkObjectResult(new _ResponseDto
                        {
                            Valid = false,
                            Validations = vals
                        });
                        return;
                    }
                }
            }

            await next();
        }


        private async Task<ValidationDto> ValidateVersion(SessionEnt session, _BaseDto dto, VersionAtt att)
        {
            if (dto == null || !dto.IsCheckVersion()) return null;
            
            VersionI info = null;

            if (att.IsIdKey)
                info = await BaseRepo.GetVersion(dto.Id, att.TableName, att.KeyField);

            else if (att.IsIntegerKey)
            {
                var nrProperty = dto.GetType().GetProperty(att.KeyField);
                if (nrProperty != null)
                {
                    int nr = (int)nrProperty.GetValue(dto)!;
                    info = await BaseRepo.GetVersion(Convert.ToInt64(nr), att.TableName, att.KeyField);
                }
            }

            else
                _log.Error("Error VersionActionFilter (1)");


            //Check if Version has changed
            if (info.Version != dto.Version)
            {
                var verX = GetLabel(session.Labels, "VerX", "This record was changed on %% and is out of date. You must reload it."); //has (%%) appended
                var verC = GetLabel(session.Labels, "VerC", "Version Conflict");

                var zone = TimeZoneInfo.ConvertTime(info.Updated, session.TimeZone);
                var ex = verX.Replace(GC.LabelParameterPrefix, zone.ToString(GC.Date_TS_Format));

                var message = new ValidationMessageDto
                {
                    Message = verC + ": " + ex,
                    IsError = true
                };
                return new ValidationDto
                {
                    Id = dto.Id,
                    Messages = new List<ValidationMessageDto> { message }
                };

            }
            return null;
        }

        private string GetLabel(Dictionary<string, string> labels, string langKey, string nullDefault)
        {
            if (labels != null && labels.ContainsKey(langKey))
                return labels[langKey];
            return nullDefault;
        }


    }
}
