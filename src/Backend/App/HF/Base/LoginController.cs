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
        private readonly MfaServiceI _mfaService;

        /// <summary>
        /// Constructor
        /// </summary>
        public LoginController(
            IServiceProvider serviceProvider,
            LoginServiceI loginService,
            MfaServiceI mfaService) : base(serviceProvider)
        {
            _loginService = loginService;
            _mfaService = mfaService;
        }

        [AllowAnonymous]
        [HttpPost("hf-login")]
        [EnableRateLimiting("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var ipAddress = GetClientIp();

            var (login, session) = await _loginService.LoginUser(ipAddress, request, false);
            ConfigureSession(session);

            var res = _loginService.ConfigureLoginResponse(request, login);

            if (!login.Response.IsValid)
                return Ok(res.ResponseDto);

            HttpContext.Response.Cookies.Append(GC.Cookie_RememberMe + request.UrlSuffix, res.EncryptedCookie, res.CookieOptions);
            return Ok(res.ResponseDto);
        }


        [AllowAnonymous]
        [HttpPost("hf-verifyMfa")]
        public async Task<IActionResult> VerifyMfa([FromBody] Common.Request.LoginRequest request)
        {
            var result = await _mfaService.VerifyMfaCode(request.Id.Value, request.MfaCode);

            if (!result)
                return Ok(_mfaService.ConfigureInvalidResponse());


            var ipAddress = GetClientIp();
            var (login, session) = await _loginService.LoginUser(ipAddress, request, true);
            ConfigureSession(session);

            var r = new _ResponseDto
            {
                Valid = result,
                Result = _loginService.ConfigureLoginResponseMfa(login)
            };
            return Ok(r);
        }

        private void ConfigureSession (SessionEnt session)
        {
            if (session == null) return;
        }

    }

}