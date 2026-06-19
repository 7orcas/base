using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Superpower.Model;

/// <summary>
/// Login controller for any client
/// When successful, a valid security token is issued (unless MFA is required, then that is the next step)
/// Created: June 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Login
{
    [IgnoreAntiforgeryToken]
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : BaseController
    {
        private readonly LoginServiceI _loginService;
        

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="labelService"></param>
        public LoginController(
            IServiceProvider serviceProvider,
            LoginServiceI loginService) : base(serviceProvider)
        {
            _loginService = loginService;
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


            //will need cross domain cookie handling if the client is on a different domain, but for now we will assume same domain
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(30),
                Path = "/"
            };

            HttpContext.Response.Cookies.Append("remember_me", "apicookie u='" + request.Username + "' p='" +  request.Password + "'", cookieOptions);

            //Don't send the next step if MFA is active as that needs to happen first
            var r = new _ResponseDto
            {
                SuccessMessage = "Login Ok",
                Valid = true,
                Result = new LoginSuccessDto { 
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
    }
}