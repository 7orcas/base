using Common.DTO.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GC = Backend.GlobalConstants;

namespace Backend.Base.Audit
{
    [Authorize]
    [PermissionAtt(GC.PerAudit)]
    [ApiController]
    [Route("api/[controller]")]
    public class AuditController : BaseController
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ServiceProvider"></param>
        public AuditController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }


        [CrudAtt(GC.CrudRead)]
        [AuditListAtt(GC.EntityTypeAudit)]
        [HttpGet("list")]
        public async Task<IActionResult> Get()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var events = await _auditService.GetEvents(session);
            var list = new List<AuditDto>();

            foreach (var e in events)
                list.Add(_auditService.Load(e));

            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = list
            };
            return Ok(r);
        }


    }
}