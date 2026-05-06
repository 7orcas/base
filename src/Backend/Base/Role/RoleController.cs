using Common.DTO.Base;
using Common.Validator.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using GC = Backend.GlobalConstants;

namespace Backend.Base.Role
{
    [Authorize]
    [PermissionAtt(GC.PerRole)]
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : BaseController
    {
        private readonly RoleServiceI _RoleService;
        private readonly PermissionInitialiseServiceI _PermissionInitialiseService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ServiceProvider"></param>
        /// <param name="RoleService"></param>
        public RoleController(IServiceProvider serviceProvider,
            RoleServiceI RoleService,
            PermissionInitialiseServiceI PermissionInitialiseService) 
            : base(serviceProvider)
        {
            _RoleService = RoleService;
            _PermissionInitialiseService = PermissionInitialiseService;
        }

        [CrudAtt(GC.CrudRead)]
        [AuditListAtt(GC.EntityTypeRole)]
        [HttpGet("userroles")]
        public async Task<IActionResult> GetUserRoles()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var roles = await _RoleService.GetUserRoles(session);
            var list = new List<UserRoleDto>();

            foreach (var m in roles)
            {
                list.Add(new UserRoleDto
                {
                    RoleId = m.RoleId,
                    orgNr = m.orgNr.HasValue? m.orgNr.Value : GC.BaseOrgNr,
                    Code = m.Code,
                    Description = m.Description,
                    Updated = m.Updated,
                    IsActive = m.IsActive,
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
        [AuditListAtt(GC.EntityTypeRole)]
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var roles = await _RoleService.GetRoles(session);
            var list = new List<RoleDto>();

            foreach (var m in roles)
                list.Add(LoadDto<RoleEnt, RoleDto>(m));

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
        /// <param name="id"></param>
        /// <returns></returns>
        [CrudAtt(GC.CrudRead)] //ToDo
        [AuditListAtt(GC.EntityTypeRole)]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetRole(long id)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var langDic = await _labelService.GetLanguageLabelDic(session.UserConfig.LangCodeCurrent, session.Org.LangLabelVariant);
            var permDic = _PermissionInitialiseService.GetPermissions();
            var permList = permDic.Values.OrderBy(v => v.Nr).ToList();

            var ent = await _RoleService.GetRole(id);
            var dto = LoadDto<RoleEnt, RoleDto>(ent);
            dto.RolePermissions = new List<RolePermissionDto>();
            var newId = -1L;

            foreach(var perm in permList)
            {
                var rp = ent.RolePermissions.Find(r => r.PermissionNr == perm.Nr);
                var lk = GetLabel(perm.LangKey, langDic);
                
                if (rp  != null)
                    dto.RolePermissions.Add(new RolePermissionDto {
                        Id = rp.Id,
                        RoleId = rp.RoleId,
                        PermissionNr = rp.PermissionNr,
                        Permission = lk,
                        Crud = rp.Crud,
                        Updated = rp.Updated,
                        IsActive = rp.IsActive,
                    });
                else
                    dto.RolePermissions.Add(new RolePermissionDto
                    {
                        Id = newId--,
                        RoleId = ent.Id,
                        PermissionNr = perm.Nr,
                        Permission = lk,
                        Crud = ""
                    });
            }

            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = dto
            };

            return Ok(r);
        }

        /// <summary>
        /// Update Roles
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudUpdate)]
        [AuditListAtt(GC.EntityTypeOrg)]
        [HttpPost("update")]
        public async Task<IActionResult> UpdateRoles([FromBody] List<RoleDto> dtos)
        {
            var session = HttpContext.Items["session"] as SessionEnt;

            //New roles require orgNr
            foreach (var dto in dtos.Where(e => e.IsNew()))
                dto.orgNr = session.Org.Nr;

            //Get current codes and updated date/time from db
            var roles = await _RoleService.GetRoles(session);
            var valFields = await Validate<RoleDto, RoleEnt, RoleVal>(dtos, roles);

            if (valFields.Count > 0) 
                return await Response(valFields);

            //Save
            var list = new List<RoleEnt>();
            var listDel = new List<RoleEnt>();
            foreach (var dto in dtos)
            {
                var ent = LoadEnt<RoleEnt, RoleDto>(dto);

                if (dto.IsDelete)
                {
                    listDel.Add(ent);
                    continue;
                }

                ent.RolePermissions = new List<RolePermissionEnt>();

                foreach (var p in dto.RolePermissions)
                    ent.RolePermissions.Add(new RolePermissionEnt
                        { 
                            RoleId = ent.Id,
                            PermissionNr = p.PermissionNr,
                            Crud = p.Crud,
                        });

                list.Add(ent);
            }
            await _RoleService.SaveRoles(list, listDel, session);

            //Audit changes
            

            var r = new _ResponseDto
            {
                SuccessMessage = "Save Ok",
                Result = dtos
            };
            return Ok(r);
        }


    }
}