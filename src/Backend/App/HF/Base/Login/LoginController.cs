using Backend.App.HF.Base.Session.Ent;
using Backend.Base.DataProtection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using AppS = Backend.App.HF.Base.Session;
using AppU = Backend.App.HF.Base.Urls;
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
        private readonly AppS.SessionServiceI _sessionService;
        private readonly AppU.UrlsServiceI _urlsService;

        /// <summary>
        /// Constructor
        /// </summary>
        public LoginController(
            IServiceProvider serviceProvider,
            LoginServiceI loginService,
            MfaServiceI mfaService,
            AppS.SessionServiceI sessionService,
            AppU.UrlsServiceI urlsService) : base(serviceProvider)
        {
            _loginService = loginService;
            _sessionService = sessionService;
            _mfaService = mfaService;
            _urlsService = urlsService;
        }

        [AllowAnonymous]
        [HttpPost("hf-login")]
        [EnableRateLimiting("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var ipAddress = GetClientIp();

            var (login, session) = await _loginService.LoginUser<SessionEntHF>(ipAddress, request, false);
            _sessionService.ConfigureSession(session);

            var (res, dto) = await _loginService.ConfigureLoginResponse(request, login);

            if (!login.Response.IsValid)
                return Ok(res.ResponseDto);

            dto.AppUrls = _urlsService.GetAppUrl();

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
            var (login, session) = await _loginService.LoginUser<SessionEntHF>(ipAddress, request, true);
            _sessionService.ConfigureSession(session);

            var dto = await _loginService.ConfigureLoginResponseMfa(request, login);
            dto.AppUrls = _urlsService.GetAppUrl();

            var r = new _ResponseDto
            {
                Valid = result,
                Result = dto
            };
            return Ok(r);
        }

    }

}