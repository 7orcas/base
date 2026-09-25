using Common.DTO.Base;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GC = Backend.GlobalConstants;

/// <summary>
/// Organisation controller
/// Created: June 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Org
{
    //[Authorize]
    [PermissionAtt(GC.PerOrg)]
    [ApiController]
    [AuditListAtt(GC.EntityTypeOrg)]
    [Route("api/[controller]")]
    public class OrgController : BaseController
    {
        private readonly OrgServiceI _orgService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ServiceProvider"></param>
        /// <param name="orgService"></param>
        public OrgController(IServiceProvider serviceProvider,
            OrgServiceI orgService) : base(serviceProvider)
        {
            _orgService = orgService;
        }

        /// <summary>
        /// Get Org entity field definitions
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudRead)]
        [AuditIgnoreAtt]
        [HttpGet("definition")]
        public async Task<IActionResult> GetDefinition()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var def = await _orgService.GetDefinition(session);

            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = def
            };
            return Ok(r);
        }

        /// <summary>
        /// Get Org list
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudReadList)]  
        [AuditListAtt(GC.CrudReadList)]
        [HttpGet("list")]
        public async Task<IActionResult> GetList()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var orgs = await _orgService.GetOrgList(session);
            var list = new List<OrgDto>();

            foreach (var org in orgs)
                list.Add(_orgService.PopulateList(session, org));
            
            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = list
            };
            return Ok(r);
        }

        /// <summary>
        /// Get Org
        /// </summary>
        /// <param name="nr"></param>
        /// <returns></returns>
        [CrudAtt(GC.CrudRead)] 
        [AuditListAtt(GC.CrudRead)]
        [HttpGet("get/{nr}")]
        public async Task<IActionResult> GetOrgByNr(int nr)
        {
            var org = await _orgService.GetOrg(nr);
            if (org == null)
            {
                return NotFound();
            }

            var session = HttpContext.Items["session"] as SessionEnt;
            var orgDto = _orgService.PopulateDto(session, org);

            var r = new _ResponseDto
            {
                SuccessMessage = "Config Ok",
                Result = orgDto
            };
            return Ok(r);
        }

        /// <summary>
        /// Update Org
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudUpdate)]
        [AuditListAtt(GC.CrudUpdate)]
        [VersionAtt("base.org", "Nr")]
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] UpdateRequest<List<OrgDto>> update)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var list = update.Updates as List<OrgDto>;

            //Validate 
            var vals = await _orgService.ValidateOrg(session, list);
            if (vals.Count > 0)
            {
                var v = new _ResponseDto
                {
                    Valid = false,
                    Validations = vals
                };
                return Ok(v);
            }

            //Do updates
            var listBefore = new List<OrgDto>();
            var listUpdated = new List<OrgDto>();
            foreach (var dto in list)
            {
                var org = await _orgService.GetOrgOrCreate(session, dto);
                if (org == null) continue;

                var beforeDto = _orgService.PopulateDto(session, org);
                beforeDto.IsDelete = dto.IsDelete;
                listBefore.Add(beforeDto);

                org = await _orgService.UpdateOrg(session, dto);
                if (org != null)
                {
                    var afterDto = _orgService.PopulateDto(session, org);
                    afterDto.Id = afterDto.Nr;
                    beforeDto.Nr = org.Nr; //link them
                    beforeDto.Id = org.Nr; //link them
                    listUpdated.Add(afterDto);
                }
            }
            return Ok(new _ResponseDto(listBefore, listUpdated));
        }

    }
}