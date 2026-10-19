using Backend.App.HF.Base.Session.Ent;
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

namespace Backend.Base.Login
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : BaseController
    {
        private readonly LoginServiceI _loginService;
        private readonly MfaServiceI _mfaService;
        private readonly SessionServiceI _sessionService;
        private readonly UrlsServiceI _urlsService;
        private readonly OrgServiceI _orgService;
        private readonly CookieProtector _cookieProtector;

        /// <summary>
        /// Constructor
        /// </summary>
        public LoginController(
            IServiceProvider serviceProvider,
            LoginServiceI loginService,
            MfaServiceI mfaService,
            SessionServiceI sessionService,
            UrlsServiceI urlsService,
            OrgServiceI orgService,
            CookieProtector cookieProtector) : base(serviceProvider)
        {
            _loginService = loginService;
            _mfaService = mfaService;
            _sessionService = sessionService;
            _urlsService = urlsService;
            _orgService = orgService;
            _cookieProtector = cookieProtector;
        }


        [AllowAnonymous]
        [HttpPost("login")]
        [EnableRateLimiting("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var ipAddress = GetClientIp();

            var (login, session) = await _loginService.LoginUser<SessionEntHF>(ipAddress, request, false);

            var (res, dto) = await _loginService.ConfigureLoginResponse(request, login);

            if (!login.Response.IsValid)
                return Ok(res.ResponseDto);

            dto.AppUrls = _urlsService.GetAppUrl();

            HttpContext.Response.Cookies.Append(GC.Cookie_RememberMe + request.UrlSuffix, res.EncryptedCookie, res.CookieOptions);
            return Ok(res.ResponseDto);
        }


        [AllowAnonymous]
        [HttpPost("verifyMfa")]
        public async Task<IActionResult> VerifyMfa([FromBody] Common.Request.LoginRequest request)
        {
            var result = await _mfaService.VerifyMfaCode(request.Id.Value, request.MfaCode);

            if (!result)
                return Ok(_mfaService.ConfigureInvalidResponse());


            var ipAddress = GetClientIp();
            var (login, session) = await _loginService.LoginUser<SessionEntHF>(ipAddress, request, true);

            var r = new _ResponseDto
            {
                Valid = result,
                Result = await _loginService.ConfigureLoginResponseMfa(request, login)
            };
            return Ok(r);
        }


        /// <summary>
        /// Gets the passed in langCode's labels
        /// The returned DTO objects contain minimal data
        /// </summary>
        /// <param name="langCode"></param>
        /// <returns></returns>
        [AllowAnonymous]
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
        [AllowAnonymous]    
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

        [AllowAnonymous]
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

        [AllowAnonymous]
        [HttpPost("resetrequest")]
        public async Task<IActionResult> ResetRequest([FromBody] ResetRequest request)
        {
            var ipAddress = GetClientIp();

            await _loginService.ResetRequest(request.Email, ipAddress, request.LangCode);

            var r = new _ResponseDto
            {
                SuccessMessage = "Reset Request",
                Valid = true,
            };
            return Ok(r);
        }

        [AllowAnonymous]
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