using Common.DTO.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GC = Backend.GlobalConstants;

namespace Backend.Base.Audit
{
    //[Authorize]
    [PermissionAtt(GC.PerAudit)]
    [ApiController]
    [AuditListAtt(GC.EntityTypeAudit)]
    [Route("api/[controller]")]
    public class AuditController : BaseController
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ServiceProvider"></param>
        public AuditController(IServiceProvider serviceProvider)
            : base(serviceProvider) { }


        [CrudAtt(GC.CrudReadList)]
        [AuditListAtt(GC.CrudReadList)]
        [HttpPost("list")]
        public async Task<IActionResult> GetList([FromBody] AuditSearch search)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var events = await _auditService.GetEvents(session, search);
            var list = new List<AuditDto>();

            foreach (var e in events)
                list.Add(_auditService.Populate(e));

            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = list
            };
            return Ok(r);
        }

        /// <summary>
        /// Get Audit record by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CrudAtt(GC.CrudRead)]
        [AuditListAtt(GC.CrudRead)]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetAuditById(long id)
        {
            var audit = await _auditService.GetById(id);
            if (audit == null)
            {
                return NotFound();
            }

            var session = HttpContext.Items["session"] as SessionEnt;
            var auditDto = _auditService.Populate(audit);
            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = auditDto
            };
            return Ok(r);
        }

    }
}