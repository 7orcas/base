using Common.DTO.Base;
using Common.Validator.Base;
using Microsoft.AspNetCore.Mvc;
using GC = Backend.GlobalConstants;

namespace Backend.CodeTemplate.Generic
{
    //[Authorize]
    [PermissionAtt(GC.PerCodeTemplate)]
    [ApiController]
    [AuditListAtt(GC.EntityTypeTemplate)]
    [Route("api/[controller]")]
    public class CodeTemplateController : BaseController
    {
        private readonly CodeTemplateServiceI _codeTemplateService;
        private const string SearchCacheKey = "CodeTemplateSearch";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ServiceProvider"></param>
        /// <param name="CodeTemplateServiceI"></param>
        public CodeTemplateController(IServiceProvider serviceProvider,
            CodeTemplateServiceI codeTemplateService,
            PermissionInitialiseServiceI PermissionInitialiseService) 
            : base(serviceProvider)
        {
            _codeTemplateService = codeTemplateService;
        }

        /// <summary>
        /// Get CodeTemplate entity field definitions
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudRead)]
        [AuditIgnoreAtt]
        [HttpGet("def")]
        public async Task<IActionResult> GetDefinition()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var def = await _codeTemplateService.GetDefinition(session);

            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = def
            };
            return Ok(r);
        }


        [CrudAtt(GC.CrudIgnore)]
        [AuditIgnoreAtt]
        [HttpGet("search")]
        public async Task<IActionResult> GetSearch()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var s = await GetUserCache<CodeTemplateSearch>(session, SearchCacheKey);
            s = s ?? new CodeTemplateSearch();
            return Ok(new _ResponseDto(s));
        }


        [CrudAtt(GC.CrudRead)]
        [AuditListAtt(GC.CrudReadList)]
        [HttpPost("list")]
        public async Task<IActionResult> GetCodeTemplates([FromBody] CodeTemplateSearch search)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var entities = await _codeTemplateService.GetCodeTemplateList(session, search);
            var list = _codeTemplateService.Populate(session, entities);

            return Ok(new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = list
            });
        }

        /// <summary>
        /// Get specific CodeTemplate
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CrudAtt(GC.CrudRead)] //ToDo
        [AuditListAtt(GC.CrudRead)]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetCodeTemplate(long id)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var entity = await _codeTemplateService.GetCodeTemplateById(id);
            var dto = _codeTemplateService.Populate(session, entity);

            return Ok(new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = dto
            });
        }

        /// <summary>
        /// Update CodeTemplates
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudUpdate)]
        [AuditListAtt(GC.CrudUpdate)]
        [VersionAtt("base.zcodetemplate")]
        [HttpPost("update")]
        public async Task<IActionResult> UpdateCodeTemplates([FromBody] UpdateRequest<List<CodeTemplateDto>> update)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var list = update.Updates as List<CodeTemplateDto>;

            //Validate 
            var vals = await _codeTemplateService.ValidateCodeTemplates(session, list);
            if (vals.Count > 0)
                return Ok(new _ResponseDto
                {
                    Valid = false,
                    Validations = vals
                });


            //Do updates
            var listBefore = new List<CodeTemplateDto>();
            var listUpdated = new List<CodeTemplateDto>();
            foreach (var dto in list)
            {
                var codeTemplate = await _codeTemplateService.GetCodeTemplateOrCreate(session, dto);
                if (codeTemplate == null) continue;

                var beforeDto = _codeTemplateService.Populate(session, codeTemplate);
                beforeDto.IsDelete = dto.IsDelete;
                listBefore.Add(beforeDto);

                codeTemplate = await _codeTemplateService.UpdateCodeTemplate(session, codeTemplate, dto);
                if (codeTemplate != null)
                {
                    beforeDto.Id = codeTemplate.Id; //link them
                    listUpdated.Add(_codeTemplateService.Populate(session, codeTemplate));
                }
            }

            if (listBefore.Count == 0)
                return Ok(new _ResponseDto
                {
                    Valid = false,
                    ErrorMessage = "No Updates"
                });

            return Ok(new _ResponseDto(listBefore, listUpdated));
        }

        /// <summary>
        /// Get a new CodeTemplate entity with default values
        /// Note it is not saved to the database yet, it is just a template for creating a new CodeTemplate dto
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudCreate)]
        [AuditListAtt(GC.CrudCreate)]
        [HttpGet("new")]
        public async Task<IActionResult> New()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var dto = await _codeTemplateService.NewCodeTemplate(session);

            return Ok(new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = dto
            });
        }

    }
}