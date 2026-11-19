using Common.DTO.Base;
using Common.Validator.Base;
using Microsoft.AspNetCore.Mvc;
using GC = Backend.GlobalConstants;

namespace Backend.App.HF.TrailGroup
{
    //[Authorize]
    [PermissionAtt(GC.PerTrailGroup)]
    [ApiController]
    [AuditListAtt(GC.EntityTypeTemplate)]
    [Route("api/[controller]")]
    public class TrailGroupController : BaseController
    {
        private readonly TrailGroupServiceI _trailGroupService;
        private const string SearchCacheKey = "TrailGroupSearch";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ServiceProvider"></param>
        /// <param name="TrailGroupServiceI"></param>
        public TrailGroupController(IServiceProvider serviceProvider,
            TrailGroupServiceI trailGroupService) 
            : base(serviceProvider)
        {
            _trailGroupService = trailGroupService;
        }

        /// <summary>
        /// Get TrailGroup entity field definitions
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudRead)]
        [AuditIgnoreAtt]
        [HttpGet("def")]
        public async Task<IActionResult> GetDefinition()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var def = await _trailGroupService.GetDefinition(session);

            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = def
            };
            return Ok(r);
        }


        /// <summary>
        /// Get TrailGroup search dto from cache or create a new one if not found
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudIgnore)]
        [AuditIgnoreAtt]
        [HttpGet("search")]
        public async Task<IActionResult> GetSearch()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var s = await GetUserCache<TrailGroupSearch>(session, SearchCacheKey);
            s = s ?? new TrailGroupSearch();
            return Ok(new _ResponseDto(s));
        }

        /// <summary>
        /// Get TrailGroups dtos 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [CrudAtt(GC.CrudRead)]
        [AuditListAtt(GC.CrudReadList)]
        [HttpPost("list")]
        public async Task<IActionResult> GetTrailGroups([FromBody] TrailGroupSearch search)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var entities = await _trailGroupService.GetTrailGroupList(session, search);
            var list = _trailGroupService.Populate(session, entities);

            return Ok(new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = list
            });
        }

        /// <summary>
        /// Get specific TrailGroup
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CrudAtt(GC.CrudRead)] //ToDo
        [AuditListAtt(GC.CrudRead)]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetTrailGroup(long id)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var entity = await _trailGroupService.GetTrailGroupById(id);
            var dto = _trailGroupService.Populate(session, entity);

            return Ok(new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = dto
            });
        }

        /// <summary>
        /// Update TrailGroups
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudUpdate)]
        [AuditListAtt(GC.CrudUpdate)]
        [VersionAtt("hf.trailGroup")]
        [HttpPost("update")]
        public async Task<IActionResult> UpdateTrailGroups([FromBody] UpdateRequest<List<TrailGroupDto>> update)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var list = update.Updates as List<TrailGroupDto>;

            //Validate 
            var vals = await _trailGroupService.ValidateTrailGroups(session, list);
            if (vals.Count > 0)
                return Ok(new _ResponseDto
                {
                    Valid = false,
                    Validations = vals
                });


            //Do updates
            var listBefore = new List<TrailGroupDto>();
            var listUpdated = new List<TrailGroupDto>();
            foreach (var dto in list)
            {
                var trailGroup = await _trailGroupService.GetTrailGroupOrCreate(session, dto);
                if (trailGroup == null) continue;

                var beforeDto = _trailGroupService.Populate(session, trailGroup);
                beforeDto.IsDelete = dto.IsDelete;
                listBefore.Add(beforeDto);

                trailGroup = await _trailGroupService.UpdateTrailGroup(session, trailGroup, dto);
                if (trailGroup != null)
                {
                    beforeDto.Id = trailGroup.Id; //link them
                    listUpdated.Add(_trailGroupService.Populate(session, trailGroup));
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
        /// Get a new TrailGroup entity with default values
        /// Note it is not saved to the database yet, it is just a template for creating a new TrailGroup dto
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudCreate)]
        [AuditListAtt(GC.CrudCreate)]
        [HttpGet("new")]
        public async Task<IActionResult> New()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var dto = await _trailGroupService.NewTrailGroup(session);

            return Ok(new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = dto
            });
        }

    }
}