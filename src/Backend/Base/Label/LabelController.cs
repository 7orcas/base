using Common.DTO.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog.Events;
using GC = Backend.GlobalConstants;

namespace Backend.Base.Label
{
    [Authorize]
    [PermissionAtt(GC.PerLabel)]
    [ApiController]
    [Route("api/[controller]")]
    public class LabelController : BaseController
    {
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ServiceProvider"></param>
        /// <param name="LabelService"></param>
        public LabelController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// Gets the passed in langCode's labels
        /// The returned DTO objects contain minimal data
        /// </summary>
        /// <param name="langCode"></param>
        /// <returns></returns>
        [CrudAtt(GC.CrudRead)]
        [AuditListAtt(GC.EntityTypeLangLabelList)]
        [HttpGet("clientlist/{langCode}/{variant}")]
        public async Task<IActionResult> GetClientLabelList(string langCode, int? variant)
        {
            var session = HttpContext.Items["session"] as SessionEnt;

            if (!IsSame(variant, session.Org.LangLabelVariant))
            {
                LogEvent(LogEventLevel.Warning, "Invalid language variant", session);
                var e = new _ResponseDto
                {
                    Valid = false,
                    ErrorMessage = "Invalid variant",
                };
                return Ok(e);
            }

            var labels = await _labelService.GetLanguageLabelList(langCode, variant);
            var list = new List<LangLabelDto>();
            
            foreach (var l in labels)
            {
                list.Add(new LangLabelDto
                {
                    Id = l.Id,
                    LangKeyCode = l.LangKeyCode,
                    Label = l.Code,
                    Tooltip = l.Tooltip
                });
            }

            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = list
            };
            return Ok(r);
        }

        /// <summary>
        /// Gets related language values the passed in label key code
        /// </summary>
        /// <param name="LangKeyCode"></param>
        /// <returns></returns>
        [CrudAtt(GC.CrudRead)]
        [AuditListAtt(GC.EntityTypeLangLabelRelated)]
        [HttpGet("relatedlist/{LangKeyCode}")]
        public async Task<IActionResult> GetRelatedLabels(string langKeyCode)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var langs = session.UserConfig.Languages;
            
            if (langs.Count == 0)
                return Ok(new _ResponseDto
                    {
                        Valid = false,
                        StatusCode = 401,
                        ErrorMessage = "Language admin not configured",
                    });
            
            var langCodes = new List<string>();
            foreach (var lc in langs)
                langCodes.Add(lc.LangCode);

            var key = await _labelService.GetLanguageKey(langKeyCode);
            var labels = await _labelService.GetRelatedLabels(langKeyCode, langCodes);
            var list = new List<LangLabelDto>();

            foreach (var lang in langs)
            {
                var dto = null as LangLabelDto;
                foreach (var l in labels)
                    if (l.LangCode == lang.LangCode)
                    {
                        dto = new LangLabelDto
                        {
                            Id = l.Id,
                            LangKeyId = l.LangKeyId,
                            Variant = l.Variant,
                            LangCode = l.LangCode,
                            Label = l.Code,
                            Tooltip = l.Tooltip,
                            Updated = l.Updated,
                            IsUpdateable = lang.IsEditable,
                        };
                        break;
                    }

                if (dto == null)
                    dto = new LangLabelDto
                    {
                        Id = GC.NewRecordId,
                        LangKeyId = key != null? key.Id : GC.NewRecordId,
                        LangCode = lang.LangCode,
                        IsUpdateable = lang.IsEditable,
                    };
                list.Add(dto);
            }

            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = list
            };
            return Ok(r);
        }


    }
}