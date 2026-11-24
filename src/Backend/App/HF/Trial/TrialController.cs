using Common.DTO.Base;
using Common.Validator.Base;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using GC = Backend.App.HF.GlobalConstantsHF;

namespace Backend.App.HF.Trial
{
    //[Authorize]
    [PermissionAtt(GC.PerTrial)]
    [ApiController]
    [AuditListAtt(GC.EntityTypeTrial)]
    [Route("api/[controller]")]
    public class TrialController : BaseController
    {
        private readonly TrialServiceI _trialService;
        private const string SearchCacheKey = "TrialSearch";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ServiceProvider"></param>
        /// <param name="TrialServiceI"></param>
        public TrialController(IServiceProvider serviceProvider,
            TrialServiceI trialService) 
            : base(serviceProvider)
        {
            _trialService = trialService;
        }

        /// <summary>
        /// Get Trial configuration definition for the front end to use when creating a new Trial
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudRead)]
        [AuditIgnoreAtt]
        [HttpGet("config")]
        public async Task<IActionResult> GetConfiguration()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var config = await _trialService.GetConfiguration(session);

            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = config
            };
            return Ok(r);
        }


        /// <summary>
        /// Get Trial entity field definitions
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudRead)]
        [AuditIgnoreAtt]
        [HttpGet("def")]
        public async Task<IActionResult> GetDefinition()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var def = await _trialService.GetDefinition(session);

            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = def
            };
            return Ok(r);
        }


        /// <summary>
        /// Get Trial search dto from cache or create a new one if not found
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudIgnore)]
        [AuditIgnoreAtt]
        [HttpGet("search")]
        public async Task<IActionResult> GetSearch()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var s = await GetUserCache<TrialSearch>(session, SearchCacheKey);
            s = s ?? new TrialSearch();
            return Ok(new _ResponseDto(s));
        }

        /// <summary>
        /// Get Trials dtos 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [CrudAtt(GC.CrudRead)]
        [AuditListAtt(GC.CrudReadList)]
        [UserCacheAtt("TrialSearch")]
        [HttpPost("list")]
        public async Task<IActionResult> GetTrials([FromBody] TrialSearch search)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var entities = await _trialService.GetTrialList(session, search);
            var list = _trialService.Populate(session, entities);

            return Ok(new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = list
            });
        }

        /// <summary>
        /// Get specific Trial
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CrudAtt(GC.CrudRead)] //ToDo
        [AuditListAtt(GC.CrudRead)]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetTrial(long id)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var entity = await _trialService.GetTrialById(id);
            var dto = _trialService.Populate(session, entity);

            return Ok(new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = dto
            });
        }

        /// <summary>
        /// Update Trials
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudUpdate)]
        [AuditListAtt(GC.CrudUpdate)]
        [VersionAtt("hf.trial")]
        [HttpPost("update")]
        public async Task<IActionResult> UpdateTrials([FromBody] UpdateRequest<List<TrialDto>> update)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var list = update.Updates as List<TrialDto>;

            //Validate 
            var vals = await _trialService.ValidateTrials(session, list);
            if (vals.Count > 0)
                return Ok(new _ResponseDto
                {
                    Valid = false,
                    Validations = vals
                });


            //Do updates
            var listBefore = new List<TrialDto>();
            var listUpdated = new List<TrialDto>();
            foreach (var dto in list)
            {
                var trial = await _trialService.GetTrialOrCreate(session, dto);
                if (trial == null) continue;

                var beforeDto = _trialService.Populate(session, trial);
                beforeDto.IsDelete = dto.IsDelete;
                listBefore.Add(beforeDto);

                trial = await _trialService.UpdateTrial(session, trial, dto);
                if (trial != null)
                {
                    beforeDto.Id = trial.Id; //link them
                    listUpdated.Add(_trialService.Populate(session, trial));
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
        /// Get a new Trial entity with default values
        /// Note it is not saved to the database yet, it is just a template for creating a new Trial dto
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudCreate)]
        [AuditListAtt(GC.CrudCreate)]
        [HttpGet("new")]
        public async Task<IActionResult> New()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var dto = await _trialService.NewTrial(session);

            return Ok(new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = dto
            });
        }

        [CrudAtt(GC.CrudCreate)]
        [AuditListAtt(GC.CrudCreate)]
        [HttpPost("starttrial")]
        public async Task<IActionResult> StartTrial([FromBody] TrialDto dto)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var info = await _trialService.StartTrial(session, dto);
                
            if (info.Status != (int)GC.TrialStatus.Running)
            {
                return Ok(new _ResponseDto
                {
                    Valid = false,
                    ErrorMessage = session.GetLabel("TrialEr2", "Trial did not start")
                });
            }

            var entity = await _trialService.GetTrialById(dto.Id);
            dto = _trialService.Populate(session, entity);
            return Ok(new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = dto
            });
        }

    }
}