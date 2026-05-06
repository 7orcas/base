using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GC = Backend.GlobalConstants;
using System.Runtime.ConstrainedExecution;
using Common.DTO.Base;

namespace Backend.Base.Permission
{
    [Authorize]
    [PermissionAtt(GC.PerPerm)]
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionController : BaseController
    {
        private readonly PermissionInitialiseServiceI _PermissionInitialiseService;
        private readonly PermissionServiceI _PermissionService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ServiceProvider"></param>
        /// <param name="PermissionService"></param>
        public PermissionController(IServiceProvider serviceProvider,
            PermissionInitialiseServiceI PermissionInitialiseService,
            PermissionServiceI PermissionService) : base(serviceProvider)
        {
            _PermissionInitialiseService = PermissionInitialiseService;
            _PermissionService = PermissionService;
        }

        [CrudAtt(GC.CrudRead)]
        [AuditListAtt(GC.EntityTypePermUser)]
        [HttpGet("userlist")]
        public async Task<IActionResult> GetUserPermissions()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var permissions = await _PermissionService.GetPermissions(session);
            var permDic = _PermissionInitialiseService.GetPermissions();
            var langDic = await _labelService.GetLanguageLabelDic(session);

            var list = new List<UserRolePermissionDto>();
            foreach (var m in permissions)
            {
                var lk = "?";
                if (permDic.ContainsKey(m.PermissionNr))
                {
                    lk = (permDic[m.PermissionNr]).LangKey;
                    lk = GetLabel(lk, langDic);
                }

                list.Add(new UserRolePermissionDto
                {
                    OrgNr = m.OrgNr,
                    Role = m.Role,
                    PermissionNr = m.PermissionNr,
                    LangKey = lk,
                    Crud = m.Crud
                });
            }

            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = list
            };
            return Ok(r);
        }

        [CrudAtt(GC.CrudRead)]
        [AuditListAtt(GC.EntityTypePermUserEffect)]
        [HttpGet("userlisteff")]
        public async Task<IActionResult> GetEffective()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var permissions = await _PermissionService.LoadEffectivePermissions(session);
            var permDic = _PermissionInitialiseService.GetPermissions();
            var langDic = await _labelService.GetLanguageLabelDic(session);

            var list = new List<PermissionDto>();

            foreach (var m in permissions)
            {
                if (!permDic.ContainsKey(m.Nr)) continue;
                var per = permDic[m.Nr];

                list.Add(new PermissionDto
                {
                    PermissionNr = per.Nr,
                    LangKey = GetLabel(per.LangKey, langDic),
                    Crud = m.Crud
                });
            }

            list = list.OrderBy(r => r.PermissionNr).ToList();

            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = list
            };
            return Ok(r);
        }

        [CrudAtt(GC.CrudRead)]
        [AuditListAtt(GC.EntityTypePermList)]
        [HttpGet("list")]
        public async Task<IActionResult> GetPermissionList()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var permDic = _PermissionInitialiseService.GetPermissions();
            var langDic = await _labelService.GetLanguageLabelDic(session);

            var list = new List<PermissionDto>();

            foreach (var perm in permDic.Values.ToList())
            {
                list.Add(new PermissionDto
                {
                    PermissionNr = perm.Nr,
                    LangKey = GetLabel(perm.LangKey, langDic),
                });
            }

            list = list.OrderBy(r => r.PermissionNr).ToList();

            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = list
            };
            return Ok(r);
        }

    }
}