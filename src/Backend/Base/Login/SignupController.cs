using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Self Registration controller for the blazor client
/// Created: July 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Login
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignupController : BaseController
    {
        private readonly LoginServiceI _loginService;
        private readonly SignupServiceI _signupService;
        private readonly OrgServiceI _orgService;
        
        /// <summary>
        /// Constructor
        /// </summary>
        public SignupController(
            IServiceProvider serviceProvider,
            LoginServiceI loginService,
            SignupServiceI signupService,
            OrgServiceI orgService) : base(serviceProvider)
        {
            _loginService = loginService;
            _signupService = signupService;
            _orgService = orgService;
        }


        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            var ipAddress = GetClientIp();
            var res = await _signupService.SignupUser(ipAddress, request.UserName, request.Email, request.Password, request.OrgNr, request.LangCode);

            var r = new _ResponseDto
            {
                SuccessMessage = "Signup process",
                Valid = res.success,
                Result = res.message
            };
            return Ok(r);
        }

        [AllowAnonymous]
        [HttpGet("signupverifyemail")]
        public async Task<IActionResult> VerifyEmail([FromQuery] int orgNr, [FromQuery] string langCode, [FromQuery] int langVariant, [FromQuery] string email)
        {
            var ipAddress = GetClientIp();
            var rtn = await _signupService.VerifyEmail(ipAddress, email, orgNr, langCode);
            return Content(rtn.message, "text/html");
        }


    }

}