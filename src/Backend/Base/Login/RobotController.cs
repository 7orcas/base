using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Not A Robot controller for the blazor client
/// Admion of keys: https://www.google.com/recaptcha/admin
/// Created: July 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Login
{
    [ApiController]
    [Route("api/[controller]")]
    public class RobotController : BaseController
    {
        private readonly RobotServiceI _robotService;
        private readonly OrgServiceI _orgService;
        
        /// <summary>
        /// Constructor
        /// </summary>
        public RobotController(
            IServiceProvider serviceProvider,
            OrgServiceI orgService,
            RobotServiceI robotService) : base(serviceProvider)
        {
            _orgService = orgService;
            _robotService = robotService;
        }


        [AllowAnonymous]
        [HttpPost("iamnotarobot")]
        public async Task<IActionResult> NotRobot([FromBody] RobotRequest request)
        {
            var ipAddress = GetClientIp();
            var res = await _robotService.Verify(ipAddress, request.CaptchaToken, request.LangCode);

            var dto = new RobotDto { 
                Message = res.message,
                Token = res.temptoken,
            };

            var r = new _ResponseDto
            {
                SuccessMessage = res.success ? res.message : null,
                ErrorMessage = !res.success ? res.message : null,
                Valid = res.success,
                Result = dto
            };
            return Ok(r);
        }


    }

}