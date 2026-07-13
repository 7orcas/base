using Backend.Base.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Serilog.Events;
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
        private readonly OrgServiceI _orgService;
        private readonly CookieProtector _cookieProtector;

        /// <summary>
        /// Constructor
        /// </summary>
        public LoginController(
            IServiceProvider serviceProvider,
            LoginServiceI loginService,
            OrgServiceI orgService,
            CookieProtector cookieProtector) : base(serviceProvider)
        {
            _loginService = loginService;
            _orgService = orgService;
            _cookieProtector = cookieProtector;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var ipAddress = GetClientIp();
            var login = await _loginService.LoginUser(ipAddress, request.UserName, request.Password, request.Org, request.SourceApplication, request.LangCode, false);
            var res = login.Response;

            if (!res.IsValid)
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
                    ",u=" + request.UserName +
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
                    TokenKey = res.IsMfaRequired ? null : res.TokenKey,
                    MainUrl = res.IsMfaRequired ? null : res.MainUrl,
                    LangCode = res.LangCode,
                    IsMfaRequired = res.IsMfaRequired,
                    IsMfaEnabled = res.IsMfaEnabled
                }
            };
            return Ok(r);
        }

        /// <summary>
        /// Gets the passed in langCode's labels
        /// The returned DTO objects contain minimal data
        /// </summary>
        /// <param name="langCode"></param>
        /// <returns></returns>
        [HttpGet("loginlabels")]
        public async Task<IActionResult> LoginLabels([FromQuery] string langCode, [FromQuery] int? variant)
        {
            var labels = await _labelService.GetLanguageLabelListForLogin(langCode, variant);
            var list = new List<LangLabelDto>();

            foreach (var l in labels)
            {
                list.Add(new LangLabelDto
                {
                    Id = l.Id,
                    LangKeyCode = l.LangKeyCode,
                    Label = l.Code,
                    Tooltip = l.Tooltip
                });
            }

            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = list
            };
            return Ok(r);
        }

        /// <summary>
        /// Get Org
        /// </summary>
        /// <param name="nr"></param>
        /// <returns></returns>
        [HttpGet("org")]
        public async Task<IActionResult> LoginOrg([FromQuery] int orgNr)
        {
            var org = await _orgService.GetOrg(orgNr);

            var r = new _ResponseDto
            {
                SuccessMessage = "Config Ok",
                Result = new OrgDto
                {
                    Nr = org.Nr,
                    Code = org.Code,
                    Description = org.Description,
                    Icon = org.Icon,
                }
            };
            return Ok(r);
        }

        [HttpGet("passwordrules")]
        public async Task<IActionResult> GetPasswordRules([FromQuery] string langCode, [FromQuery] int orgNr)
        {
            var rules = await _orgService.GetPasswordRules(langCode, orgNr);
            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = rules,
                Valid = true,
            };
            return Ok(r);
        }

        [HttpGet("resetrequest")]
        public async Task<IActionResult> ResetRequest([FromQuery] string email)
        {
            var ipAddress = GetClientIp();

            var success = await _loginService.ResetRequest(email, ipAddress);

            var r = new _ResponseDto
            {
                SuccessMessage = "Reset Request",
                Valid = success,
            };
            return Ok(r);
        }

        [HttpPost("resetaction")]
        public async Task<IActionResult> ResetAction([FromBody] LoginResetActionDto action)
        {
            var ipAddress = GetClientIp();

            var rtn = await _loginService.ResetAction(action.Password, action.Token, ipAddress, action.OrgNr, action.LangCode);

            var r = new _ResponseDto
            {
                SuccessMessage = "Reset Action",
                Result = rtn.message,
                Valid = rtn.success,
            };
            return Ok(r);
        }

    }

}