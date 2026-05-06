using Common.DTO;
using Common.Validator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog.Events;
using System.Diagnostics;
using GC = Backend.GlobalConstants;

namespace Backend.Base
{
    public abstract partial class BaseController : ControllerBase
    {
        protected readonly Serilog.ILogger _log;
        protected AuditServiceI _auditService;
        protected LabelServiceI _labelService;

        public BaseController(IServiceProvider serviceProvider)
        {
            _log = Log.Logger;

            // Create a scoped service provider
            using var scope = serviceProvider.CreateScope();
            _auditService = scope.ServiceProvider.GetRequiredService<AuditServiceI>();
            _labelService = scope.ServiceProvider.GetRequiredService<LabelServiceI>();
        }

        protected string GetLabel (string langKey, Dictionary<string, LangLabel> labels)
        {
            if (labels.ContainsKey(langKey)) 
                return (labels[langKey]).Code;
            return langKey;
        }

        protected string GetLabel(string langKey, Dictionary<string, string> labels)
        {
            if (labels.ContainsKey(langKey))
                return labels[langKey];
            return langKey;
        }

        protected bool IsSame (int? i1, int? i2)
        {
            if (i1 == null && i2 == null) return true;
            if (i1 == null || i2 == null) return false;
            return i1 == i2;
        }

        protected T LoadDto<E, T>(BaseEntity<E> e) where T : _BaseFieldsDto<T>, new()
        {
            var dto = new T();
            dto.Id = e.Id;
            dto.orgNr = e.OrgNr;
            dto.Code = e.Code;
            dto.Description = e.Description;
            dto.Updated = e.Updated;
            dto.IsActive = e.IsActive;
            return dto;
        }

        protected E LoadEnt<E, D>(_BaseFieldsDto<D> dto)
            where E : BaseEntity<E>, new()
            where D : _BaseFieldsDto<D>
        {
            var ent = new E();
            ent.Id = dto.Id;
            ent.OrgNr = dto.orgNr;
            ent.Code = dto.Code;
            ent.Description = dto.Description;
            ent.Updated = dto.Updated;
            ent.IsActive = dto.IsActive;
            return ent;
        }

        public async Task<IActionResult> Response(List<ValDto> validations)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var langDic = await _labelService.GetLangCodeDic(session);

            var x = new _ResponseDto
            {
                Valid = false,
                StatusCode = GC.StatusCodeUnProcessable,
                ErrorMessage = GetLabel("InvR", langDic),
                Result = validations
            };
            return Ok(x);
        }

        protected void LogEvent(LogEventLevel level, string message, SessionEnt sessionEnt)
        {
            var logstring = "";
            try
            {
                var stackFrame = new StackTrace().GetFrame(1); // 0 = current method, 1 = caller
                var method = stackFrame.GetMethod();
                logstring = method.DeclaringType?.FullName;
               // logstring += "." + method.DeclaringType?.Name;
            }
            catch { }

            if (sessionEnt != null)
                logstring += ", SessionKey: " + sessionEnt.Key;

            logstring = message + ": " + logstring;

            if (level == LogEventLevel.Debug)
                _log.Debug(logstring);

            if (level == LogEventLevel.Error)
                _log.Error(logstring);

            if (level == LogEventLevel.Warning)
                _log.Warning(logstring);
        }

    }
}