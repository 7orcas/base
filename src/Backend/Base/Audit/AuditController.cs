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


        [CrudAtt(GC.CrudIgnore)]
        [AuditIgnoreAtt]
        [HttpGet("search")]
        public async Task<IActionResult> GetSearch()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var s = await GetUserCache<AuditSearch>(session, "AuditSearch");
            s = s ?? new AuditSearch();
            s.ShowActive = false;
            return Ok(new _ResponseDto(s));
        }


        [CrudAtt(GC.CrudReadList)]
        [AuditListAtt(GC.CrudReadList)]
        [UserCacheAtt("AuditSearch")]
        [HttpPost("list")]
        public async Task<IActionResult> GetList([FromBody] AuditSearch search)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            _auditService.ConfigureSearch(session, search);
            var events = await _auditService.GetEvents(session, search);
            var list = new List<AuditDto>();

            foreach (var e in events)
                list.Add(_auditService.Populate(e));

            return Ok(new _ResponseDto(list));
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
            var session = HttpContext.Items["session"] as SessionEnt;
            var audit = await _auditService.GetById(session, id);
            if (audit == null)
            {
                return NotFound();
            }

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