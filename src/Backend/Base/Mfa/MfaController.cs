using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Multifactor authentication controller for any client
/// When successful, a valid security token is issued 
/// Created: June 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Mfa
{
    [Route("api/[controller]")]
    public class MfaController : BaseController
    {
        private readonly MfaServiceI _MfaService;
        private readonly LoginServiceI _loginService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ServiceProvider"></param>
        /// <param name="MfaService"></param>
        public MfaController(IServiceProvider serviceProvider,
            MfaServiceI MfaService,
            LoginServiceI loginService) : base(serviceProvider)
        {
            _MfaService = MfaService;
            _loginService = loginService;
        }

        /// <summary>
        /// Get client setup for Mfa
        /// This is a mix of organisation and user account Mfa
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("SetupMfa")]
        public async Task<IActionResult> SetupMfa([FromBody] Common.Request.LoginRequest loginRequest)
        {
            var key = await _MfaService.SetupMfa(loginRequest.Id.Value, loginRequest.UserName);

            if (key == null)
                return Ok(new _ResponseDto
                {
                    Valid = false,
                });
           
            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = new MfaSetupDto
                {
                    SharedKey = key.SharedKey,
                    QrCodeUri = key.QrCodeUri
                }
            };
            return Ok(r);

        }

        [AllowAnonymous]
        [HttpPost("VerifyMfa")]
        public async Task<IActionResult> VerifyMfa([FromBody] Common.Request.LoginRequest request) 
        {
            var result = await _MfaService.VerifyMfaCode(request.Id.Value, request.MfaCode);
            
            if (!result)
                return Ok(new _ResponseDto
                {
                    Valid = false,
                    ErrorMessage = "Invalid Mfa code"
                });
        

            var ipAddress = GetClientIp();
            var login = await _loginService.LoginUser(ipAddress, request.UserName, request.Password, request.Org, request.SourceApplication, request.LangCode, true);
            var res = login.Response;

            LoginSuccessDto dto = null;

            if (result)
                dto = new LoginSuccessDto
                {
                    Id = login.Id,
                    TokenKey = res.TokenKey,
                    MainUrl = res.MainUrl,
                    LangCode = res.LangCode,
                    IsMfaRequired = res.IsMfaRequired,
                    IsMfaEnabled = res.IsMfaEnabled
                };

            var r = new _ResponseDto
            {
                Valid = result,
                Result = dto
            };
            return Ok(r);
        }

    }
}