using Backend.Base.DataProtection;
using Microsoft.AspNetCore.Mvc;
using GC = Backend.GlobalConstants;

/// <summary>
/// Login controller for any client
/// When successful, a valid security token is issued (unless MFA is required, then that is the next step)
/// Created: June 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Login
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : BaseController
    {
        private readonly LoginServiceI _loginService;
        private readonly CookieProtector _cookieProtector;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="labelService"></param>
        public LoginController(
            IServiceProvider serviceProvider,
            LoginServiceI loginService,
            CookieProtector cookieProtector) : base(serviceProvider)
        {
            _loginService = loginService;
            _cookieProtector = cookieProtector;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var ipAddress = GetClientIp();
            var login = await _loginService.LoginUser(ipAddress, request.Username, request.Password, request.Org, request.SourceApplication, request.LangCode, false);
            var res = login.Response;

            if (!res.Valid)
                return Ok(new _ResponseDto
                {
                    Valid = false,
                    ErrorMessage = res.ErrorMessage,
                });

            //Remember me cookie - only if requested and login is successful.
            //This is used to pre-populate the login form for the user, it is not a security risk as it does not contain any token or similar, just the username and org for pre-population
            var cookie = "";
            if (request.RememberMe)
            {
                cookie = "V=1" +
                    ",x=" + request.UrlSuffix +
                    ",u=" + request.Username +
                    ",p=" + request.Password +
                    ",o=" + request.Org +
                    ",l=" + request.LangCode;
            }

            var encryptedCookie = _cookieProtector.Encrypt(cookie);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(30),
                Path = "/"
            };
            HttpContext.Response.Cookies.Append(GC.Cookie_RememberMe + request.UrlSuffix, encryptedCookie, cookieOptions);


            //Don't send the next step if MFA is active as that needs to happen first
            var r = new _ResponseDto
            {
                SuccessMessage = "Login Ok",
                Valid = true,
                Result = new LoginSuccessDto
                {
                    Id = login.Id,
                    TokenKey = res.MfaRequired ? null : res.TokenKey,
                    MainUrl = res.MfaRequired ? null : res.MainUrl,
                    LangCode = res.LangCode,
                    MfaRequired = res.MfaRequired,
                    MfaEnabled = res.MfaEnabled
                }
            };
            return Ok(r);
        }

        [HttpGet("resetrequest")]
        public async Task<IActionResult> ResetRequest([FromQuery] string email)
        {
            var ipAddress = GetClientIp();

            var success = _loginService.ResetRequest(email, ipAddress);

            var r = new _ResponseDto
            {
                SuccessMessage = "Reset Request Ok",
                Valid = true,
            };
            return Ok(r);
        }

        [HttpGet("resetaction")]
        public async Task<IActionResult> ResetAction([FromQuery] string token)
        {
            var ipAddress = GetClientIp();

            var success = _loginService.ResetAction(token, ipAddress);

            var r = new _ResponseDto
            {
                SuccessMessage = "Reset Action Ok",
                Valid = true,
            };
            return Ok(r);
        }

    }

}