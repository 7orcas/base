using Backend.Base.DataProtection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using GC = Backend.GlobalConstants;

/// <summary>
/// Login controller for any client
/// When successful, a valid security token is issued (unless MFA is required, then that is the next step)
/// Created: June 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.App.HF.Base.Login
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : BaseController
    {
        private readonly LoginServiceI _loginService;

        /// <summary>
        /// Constructor
        /// </summary>
        public LoginController(
            IServiceProvider serviceProvider,
            LoginServiceI loginService) : base(serviceProvider)
        {
            _loginService = loginService;
        }

        [AllowAnonymous]
        [HttpPost("hf-login")]
        [EnableRateLimiting("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var ipAddress = GetClientIp();
            var login = await _loginService.LoginUser(ipAddress, request, false);
            var res = _loginService.ConfigureLoginResponse(request, login);

            if (!login.Response.IsValid)
                return Ok(res.ResponseDto);

            HttpContext.Response.Cookies.Append(GC.Cookie_RememberMe + request.UrlSuffix, res.EncryptedCookie, res.CookieOptions);
            return Ok(res.ResponseDto);
        }

       

    }

}