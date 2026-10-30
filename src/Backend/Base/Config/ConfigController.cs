using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using GC = Backend.GlobalConstants;

/// <summary>
/// Configuration is a combination of the org config and the user settings.
/// It can be used by the client to configuration options and functionality.
/// Created: May 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Config
{
    [PermissionAtt(GC.PerConfig)]
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigController : BaseController
    {
        private readonly ConfigServiceI _ConfigService;
        private readonly UserCacheServiceI _cacheService;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ServiceProvider"></param>
        /// <param name="ConfigService"></param>
        public ConfigController(IServiceProvider serviceProvider,
            ConfigServiceI ConfigService,
            UserCacheServiceI cacheService) : base(serviceProvider)
        {
            _ConfigService = ConfigService;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Get Org entity field definitions
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudIgnore)]
        [AuditIgnoreAtt]
        [HttpGet("def")]
        public async Task<IActionResult> GetDefinition()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var def = await _ConfigService.GetDefinition(session);

            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = def
            };
            return Ok(r);
        }


        /// <summary>
        /// Get client config
        /// This is a mix of organisation and user account config
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudIgnore)]
        [HttpGet("config")]
        public async Task<IActionResult> GetConfig()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var dto = await _ConfigService.PopulateDto(session);

            return Ok(new _ResponseDto
            {
                SuccessMessage = "Config Ok",
                Result = dto
            });
        }

        [CrudAtt(GC.CrudIgnore)]
        [AuditIgnoreAtt]
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] UpdateRequest<List<ConfigDto>> update)
        {
            var session = HttpContext.Items["session"] as SessionEnt;

            foreach (var config in update.Updates)
            {
                var auditJson = JsonSerializer.Serialize(
                                    config,
                                    new JsonSerializerOptions
                                    {
                                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                                    });

                await _cacheService.SaveCache(session, _ConfigService.GetUserCacheKey(config.Org.Nr), auditJson);
               
            }

            return Ok(_ResponseDto.OK());
        }


    }
}