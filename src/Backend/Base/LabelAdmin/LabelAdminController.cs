using Microsoft.AspNetCore.Mvc;
using GC = Backend.GlobalConstants;

namespace Backend.Base.LabelAdmin
{
    [PermissionAtt(GC.PerLabelAdmin)]
    [ApiController]
    [Route("api/[controller]")]
    public class LabelAdminController : BaseController
    {
        private readonly LabelAdminServiceI _labelAdminService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ServiceProvider"></param>
        /// <param name="LabelAdminService"></param>
        public LabelAdminController(IServiceProvider serviceProvider,
            LabelAdminServiceI labelAdminService) : base(serviceProvider)
        {
            _labelAdminService = labelAdminService;
        }

        [CrudAtt(GC.CrudIgnore)]
        [AuditIgnoreAtt]
        [HttpGet("search")]
        public async Task<IActionResult> GetSearch()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var s = await GetUserCache<LabelSearch>(session, "LabelSearch");
            s = s ?? new LabelSearch();
            return Ok(new _ResponseDto(s));
        }

        /// <summary>
        /// Gets lang codes
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudRead)]
        [AuditListAtt(GC.EntityTypeLangCode)]
        [HttpPost("codelist")]
        public async Task<IActionResult> GetLangCodeList([FromBody] LabelSearch search)
        {
            var session = HttpContext.Items["session"] as SessionEnt;

            var codes = await _labelAdminService.GetLangCodeList();
            var list = _labelAdminService.PopulatCodes(codes);

            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = list
            };
            return Ok(r);
        }

        /// <summary>
        /// Gets lang keys
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudRead)]
        [AuditListAtt(GC.EntityTypeLangKey)]
        [HttpPost("keylist")]
        public async Task<IActionResult> GetLangKeyList([FromBody] LabelSearch search)
        {
            var session = HttpContext.Items["session"] as SessionEnt;

            var keys = await _labelAdminService.GetLangKeyList();
            var list = _labelAdminService.PopulateKeys(keys);

            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = list
            };
            return Ok(r);
        }

        /// <summary>
        /// Gets lang labels
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudRead)]
        [AuditListAtt(GC.EntityTypeLangLabelList)]
        [HttpPost("labellist")]
        public async Task<IActionResult> GetLangLabelList([FromBody] LabelSearch search)
        {
            var session = HttpContext.Items["session"] as SessionEnt;

            var labels = await _labelAdminService.GetLangLabelList(search.LangCode);
            var list = _labelAdminService.PopulateLabels(labels);

            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = list
            };
            return Ok(r);
        }

    }
}