using Common.DTO.Base;
using Common.Validator.Base;
using Microsoft.AspNetCore.Mvc;
using GC = Backend.GlobalConstants;

namespace Backend.App.HF.TrialGroup
{
    //[Authorize]
    [PermissionAtt(GC.PerTrialGroup)]
    [ApiController]
    [AuditListAtt(GC.EntityTypeTrialGroup)]
    [Route("api/[controller]")]
    public class TrialGroupController : BaseController
    {
        private readonly TrialGroupServiceI _trialGroupService;
        private const string SearchCacheKey = "TrialGroupSearch";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ServiceProvider"></param>
        /// <param name="TrialGroupServiceI"></param>
        public TrialGroupController(IServiceProvider serviceProvider,
            TrialGroupServiceI trialGroupService) 
            : base(serviceProvider)
        {
            _trialGroupService = trialGroupService;
        }

        /// <summary>
        /// Get TrialGroup entity field definitions
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudRead)]
        [AuditIgnoreAtt]
        [HttpGet("def")]
        public async Task<IActionResult> GetDefinition()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var def = await _trialGroupService.GetDefinition(session);

            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = def
            };
            return Ok(r);
        }


        /// <summary>
        /// Get TrialGroup search dto from cache or create a new one if not found
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudIgnore)]
        [AuditIgnoreAtt]
        [HttpGet("search")]
        public async Task<IActionResult> GetSearch()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var s = await GetUserCache<TrialGroupSearch>(session, SearchCacheKey);
            s = s ?? new TrialGroupSearch();
            return Ok(new _ResponseDto(s));
        }

        /// <summary>
        /// Get TrialGroups dtos 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [CrudAtt(GC.CrudRead)]
        [AuditListAtt(GC.CrudReadList)]
        [HttpPost("list")]
        public async Task<IActionResult> GetTrialGroups([FromBody] TrialGroupSearch search)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var entities = await _trialGroupService.GetTrialGroupList(session, search);
            var list = _trialGroupService.Populate(session, entities);

            return Ok(new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = list
            });
        }

        /// <summary>
        /// Get specific TrialGroup
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CrudAtt(GC.CrudRead)] //ToDo
        [AuditListAtt(GC.CrudRead)]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetTrialGroup(long id)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var entity = await _trialGroupService.GetTrialGroupById(id);
            var dto = _trialGroupService.Populate(session, entity);

            return Ok(new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = dto
            });
        }

        /// <summary>
        /// Update TrialGroups
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudUpdate)]
        [AuditListAtt(GC.CrudUpdate)]
        [VersionAtt("hf.trialGroup")]
        [HttpPost("update")]
        public async Task<IActionResult> UpdateTrialGroups([FromBody] UpdateRequest<List<TrialGroupDto>> update)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var list = update.Updates as List<TrialGroupDto>;

            //Validate 
            var vals = await _trialGroupService.ValidateTrialGroups(session, list);
            if (vals.Count > 0)
                return Ok(new _ResponseDto
                {
                    Valid = false,
                    Validations = vals
                });


            //Do updates
            var listBefore = new List<TrialGroupDto>();
            var listUpdated = new List<TrialGroupDto>();
            foreach (var dto in list)
            {
                var trialGroup = await _trialGroupService.GetTrialGroupOrCreate(session, dto);
                if (trialGroup == null) continue;

                var beforeDto = _trialGroupService.Populate(session, trialGroup);
                beforeDto.IsDelete = dto.IsDelete;
                listBefore.Add(beforeDto);

                trialGroup = await _trialGroupService.UpdateTrialGroup(session, trialGroup, dto);
                if (trialGroup != null)
                {
                    beforeDto.Id = trialGroup.Id; //link them
                    listUpdated.Add(_trialGroupService.Populate(session, trialGroup));
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
        /// Get a new TrialGroup entity with default values
        /// Note it is not saved to the database yet, it is just a template for creating a new TrialGroup dto
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudCreate)]
        [AuditListAtt(GC.CrudCreate)]
        [HttpGet("new")]
        public async Task<IActionResult> New()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var dto = await _trialGroupService.NewTrialGroup(session);

            return Ok(new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = dto
            });
        }

    }
}