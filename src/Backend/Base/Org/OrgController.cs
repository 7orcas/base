using Common.DTO.Base;
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
    [Authorize]
    [PermissionAtt(GC.PerOrg)]
    [ApiController]
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

        [CrudAtt(GC.CrudIgnore)]  //ToDo
        [AuditListAtt(GC.EntityTypeOrg)]
        [HttpGet("list")]
        public async Task<IActionResult> Get()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var orgs = await _orgService.GetOrgList();
            var list = new List<OrgDto>();

            foreach (var org in orgs)
            {
                list.Add(new OrgDto
                    {
                        Id = org.Nr,
                        Code = org.Code,
                        Description = org.Description,
                        Updated = org.Updated,
                        IsActive = org.IsActive,
                    }
                );
            }

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
        [CrudAtt(GC.CrudIgnore)] //ToDo
        [AuditListAtt(GC.EntityTypeOrg)]
        [HttpGet("get/{nr}")]
        public async Task<IActionResult> GetOrg(int nr)
        {
            var org = await _orgService.GetOrg(nr);
            var enc = org.Encoding;

            var langDtos = new List<OrgLangDto>();
            foreach (var lang in enc.Languages)
            {
                langDtos.Add(new OrgLangDto
                {
                    LangCode = lang.LangCode,
                    IsReadonly = lang.IsReadonly,
                    IsEditable = lang.IsEditable,
                });
            }

            var r = new _ResponseDto
            {
                SuccessMessage = "Config Ok",
                Result = new OrgDto
                {
                    Id = org.Nr,
                    Code = org.Code,
                    Description = org.Description,
                    Updated = org.Updated,
                    IsActive = org.IsActive,
                    LangCode = org.LangCode,
                    LangLabelVariant = org.LangLabelVariant,
                    Languages = langDtos,

                    PasswordRule = new PasswordRuleDto
                    {
                        MaxNumberLoginAttempts = enc.MaxNumberLoginAttempts,
                        MinLength = enc.PasswordRule.MinLength,
                        MaxLength = enc.PasswordRule.MaxLength,
                        IsMixedCase = enc.PasswordRule.IsMixedCase,
                        IsNonLetter = enc.PasswordRule.IsNonLetter,
                        IsNumber = enc.PasswordRule.IsNumber,
                    }
                }
            };
            return Ok(r);
        }

        /// <summary>
        /// Update Org
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudUpdate)]
        [AuditListAtt(GC.EntityTypeOrg)]
        [HttpPost("update")]
        public async Task<IActionResult> UpdateOrg([FromBody] OrgDto dto)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var langs = new List<Language>();
            foreach (var langDto in dto.Languages)
            {
                langs.Add(new Language
                {
                    LangCode = langDto.LangCode,
                    IsReadonly = langDto.IsReadonly,
                    IsEditable = langDto.IsEditable,
                });
            }

            var pw = new PasswordRule() {
                MinLength = dto.PasswordRule.MinLength,
                MaxLength = dto.PasswordRule.MaxLength,
                IsMixedCase = dto.PasswordRule.IsMixedCase,
                IsNonLetter = dto.PasswordRule.IsNonLetter,
                IsNumber = dto.PasswordRule.IsNumber,
            };

            var org = new OrgEnt
            {
                Nr = dto.Id,
                Code = dto.Code,
                Description = dto.Description,
                Updated = dto.Updated,
                IsActive = dto.IsActive,
                LangCode = dto.LangCode,
                LangLabelVariant = dto.LangLabelVariant,
            };
            org.Encoding = new OrgEnc
            {
                MaxNumberLoginAttempts = dto.PasswordRule.MaxNumberLoginAttempts,
                Languages = langs,
                PasswordRule = pw,
            };

            await _orgService.UpdateOrg(org);

            var r = new _ResponseDto
            {
                SuccessMessage = "Save Ok",
                Result = dto
            };
            return Ok(r);
        }

    }
}