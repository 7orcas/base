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

        public LoginOptionController(
            LoginOptionServiceI loginOptionService,
            OrgServiceI orgService)
        {
            _loginOptionService = loginOptionService;
        }


        /// <summary>
        /// Get Login Options for a user (e.g., languages, organisations)
        /// </summary>
        /// <param name="urlSuffix"></param>
        /// <returns></returns>
        [CrudAtt(GC.CrudIgnore)] 
        [HttpGet("get/{urlSuffix}")]
        public async Task<IActionResult> LoginOptions(string urlSuffix)
        {
            var options = await _loginOptionService.GetLoginOptions(urlSuffix);

            if (!options.IsActive)
                return Ok(new _ResponseDto
                {
                    Valid = false,
                    ErrorMessage = "NAuthA",
                });


            var dto = await _loginOptionService.InitialiseLoginOptions(options);
            var cookieValue = HttpContext.Request.Cookies["remember_me"];
            
            
            dto.RememberMeCookie = cookieValue;

            var r = new _ResponseDto
            {
                SuccessMessage = "Login Options Ok",
                Result = dto
            };

            return Ok(r);
        }
    }
}