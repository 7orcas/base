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

        /// <summary>
        /// Gets lang codes
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudRead)]
        [AuditListAtt(GC.EntityTypeLangLabelList)]
        [HttpGet("codelist")]
        public async Task<IActionResult> GetLangCodeList()
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
        /// Gets lang labels
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudRead)]
        [AuditListAtt(GC.EntityTypeLangLabelList)]
        [HttpGet("labellist/{langCode}")]
        public async Task<IActionResult> GetLangLabelList(string langCode)
        {
            var session = HttpContext.Items["session"] as SessionEnt;

            var labels = await _labelAdminService.GetLangLabelList(langCode);
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