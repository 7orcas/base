using Common.DTO.Base;
using Common.Validator.Base;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using GC = Backend.App.HF.GlobalConstantsHF;

namespace Backend.App.HF.Trail
{
    //[Authorize]
    [PermissionAtt(GC.PerTrail)]
    [ApiController]
    [AuditListAtt(GC.EntityTypeTrail)]
    [Route("api/[controller]")]
    public class TrailController : BaseController
    {
        private readonly TrailServiceI _trailService;
        private const string SearchCacheKey = "TrailSearch";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ServiceProvider"></param>
        /// <param name="TrailServiceI"></param>
        public TrailController(IServiceProvider serviceProvider,
            TrailServiceI trailService) 
            : base(serviceProvider)
        {
            _trailService = trailService;
        }

        /// <summary>
        /// Get Trail configuration definition for the front end to use when creating a new Trail
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudRead)]
        [AuditIgnoreAtt]
        [HttpGet("config")]
        public async Task<IActionResult> GetConfiguration()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var config = await _trailService.GetConfiguration(session);

            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = config
            };
            return Ok(r);
        }


        /// <summary>
        /// Get Trail entity field definitions
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudRead)]
        [AuditIgnoreAtt]
        [HttpGet("def")]
        public async Task<IActionResult> GetDefinition()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var def = await _trailService.GetDefinition(session);

            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = def
            };
            return Ok(r);
        }


        /// <summary>
        /// Get Trail search dto from cache or create a new one if not found
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudIgnore)]
        [AuditIgnoreAtt]
        [HttpGet("search")]
        public async Task<IActionResult> GetSearch()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var s = await GetUserCache<TrailSearch>(session, SearchCacheKey);
            s = s ?? new TrailSearch();
            return Ok(new _ResponseDto(s));
        }

        /// <summary>
        /// Get Trails dtos 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [CrudAtt(GC.CrudRead)]
        [AuditListAtt(GC.CrudReadList)]
        [UserCacheAtt("TrailSearch")]
        [HttpPost("list")]
        public async Task<IActionResult> GetTrails([FromBody] TrailSearch search)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var entities = await _trailService.GetTrailList(session, search);
            var list = _trailService.Populate(session, entities);

            return Ok(new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = list
            });
        }

        /// <summary>
        /// Get specific Trail
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CrudAtt(GC.CrudRead)] //ToDo
        [AuditListAtt(GC.CrudRead)]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetTrail(long id)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var entity = await _trailService.GetTrailById(id);
            var dto = _trailService.Populate(session, entity);

            return Ok(new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = dto
            });
        }

        /// <summary>
        /// Update Trails
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudUpdate)]
        [AuditListAtt(GC.CrudUpdate)]
        [VersionAtt("hf.trail")]
        [HttpPost("update")]
        public async Task<IActionResult> UpdateTrails([FromBody] UpdateRequest<List<TrailDto>> update)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var list = update.Updates as List<TrailDto>;

            //Validate 
            var vals = await _trailService.ValidateTrails(session, list);
            if (vals.Count > 0)
                return Ok(new _ResponseDto
                {
                    Valid = false,
                    Validations = vals
                });


            //Do updates
            var listBefore = new List<TrailDto>();
            var listUpdated = new List<TrailDto>();
            foreach (var dto in list)
            {
                var trail = await _trailService.GetTrailOrCreate(session, dto);
                if (trail == null) continue;

                var beforeDto = _trailService.Populate(session, trail);
                beforeDto.IsDelete = dto.IsDelete;
                listBefore.Add(beforeDto);

                trail = await _trailService.UpdateTrail(session, trail, dto);
                if (trail != null)
                {
                    beforeDto.Id = trail.Id; //link them
                    listUpdated.Add(_trailService.Populate(session, trail));
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
        /// Get a new Trail entity with default values
        /// Note it is not saved to the database yet, it is just a template for creating a new Trail dto
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudCreate)]
        [AuditListAtt(GC.CrudCreate)]
        [HttpGet("new")]
        public async Task<IActionResult> New()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var dto = await _trailService.NewTrail(session);

            return Ok(new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = dto
            });
        }

        [CrudAtt(GC.CrudCreate)]
        [AuditListAtt(GC.CrudCreate)]
        [HttpPost("starttrail")]
        public async Task<IActionResult> StartTrail([FromBody] TrailDto dto)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var info = await _trailService.StartTrail(session, dto);
                
            if (info.Status != (int)GC.TrailStatus.Running)
            {
                return Ok(new _ResponseDto
                {
                    Valid = false,
                    ErrorMessage = session.GetLabel("TrailEr2", "Trail did not start")
                });
            }

            var entity = await _trailService.GetTrailById(dto.Id);
            dto = _trailService.Populate(session, entity);
            return Ok(new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = dto
            });
        }

    }
}