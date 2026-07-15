using Backend.Base.DataProtection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GC = Backend.GlobalConstants;

/// <summary>
/// Login option controller for any client
/// Provides various login options (e.g., languages, organisations)
/// Created: June 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Login
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginOptionController : ControllerBase
    {
        private readonly LoginOptionServiceI _loginOptionService;
        private readonly CookieProtector _cookieProtector;

        public LoginOptionController(
            LoginOptionServiceI loginOptionService,
            OrgServiceI orgService,
            CookieProtector cookieProtector)
        {
            _loginOptionService = loginOptionService;
            _cookieProtector = cookieProtector;
        }


        /// <summary>
        /// Get Login Options for a user (e.g., languages, organisations)
        /// </summary>
        /// <param name="urlSuffix"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [CrudAtt(GC.CrudIgnore)] 
        [HttpGet("get/{urlSuffix}")]
        public async Task<IActionResult> LoginOptions([FromRoute] string urlSuffix, [FromQuery] string? encryptedCookie)
        {
            var options = await _loginOptionService.GetLoginOptions(urlSuffix);

            if (!options.IsActive)
                return Ok(new _ResponseDto
                {
                    Valid = false,
                    ErrorMessage = "NAuthA",
                });

            var dto = await _loginOptionService.InitialiseLoginOptions(options);
            dto.RememberMeCookie = _cookieProtector.Decrypt(encryptedCookie);

            var r = new _ResponseDto
            {
                SuccessMessage = "Login Options Ok",
                Result = dto
            };

            return Ok(r);
        }
    }
}