using Backend.Base.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Serilog.Events;
using GC = Backend.GlobalConstants;

/// <summary>
/// Self Registration controller 
/// Created: June 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Registration
{
    [ApiController]
    [Route("api/[controller]")]
    public class SelfRegistrationController : BaseController
    {
        private readonly RegistrationServiceI _RegistrationService;
        private readonly OrgServiceI _orgService;
        private readonly CookieProtector _cookieProtector;

        /// <summary>
        /// Constructor
        /// </summary>
        public SelfRegistrationController(
            IServiceProvider serviceProvider,
            RegistrationServiceI RegistrationService,
            OrgServiceI orgService,
            CookieProtector cookieProtector) : base(serviceProvider)
        {
            _RegistrationService = RegistrationService;
            _orgService = orgService;
            _cookieProtector = cookieProtector;
        }


        [HttpPost("SelfRegistration")]
        public async Task<IActionResult> SelfRegistration()
        {
            var ipAddress = GetClientIp();
           

            var r = new _ResponseDto
            {
                SuccessMessage = "Registration Ok",
                Valid = true,
                Result = "FixMe"
            };
            return Ok(r);
        }

    }

}