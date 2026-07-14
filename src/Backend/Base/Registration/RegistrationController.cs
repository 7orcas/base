using Backend.Base.DataProtection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog.Events;
using GC = Backend.GlobalConstants;

/// <summary>
/// Registration controller 
/// Created: June 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Registration
{
    [Authorize]
    [PermissionAtt(GC.PerReg)]
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationController : BaseController
    {
        private readonly RegistrationServiceI _RegistrationService;
        private readonly OrgServiceI _orgService;
        private readonly CookieProtector _cookieProtector;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="labelService"></param>
        public RegistrationController(
            IServiceProvider serviceProvider,
            RegistrationServiceI RegistrationService,
            OrgServiceI orgService,
            CookieProtector cookieProtector) : base(serviceProvider)
        {
            _RegistrationService = RegistrationService;
            _orgService = orgService;
            _cookieProtector = cookieProtector;
        }


        [HttpPost("Registration")]
        public async Task<IActionResult> Registration()
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