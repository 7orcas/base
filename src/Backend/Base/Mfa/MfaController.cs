using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OtpNet;
using GC = Backend.GlobalConstants;

/// <summary>
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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ServiceProvider"></param>
        /// <param name="MfaService"></param>
        public MfaController(IServiceProvider serviceProvider,
            MfaServiceI MfaService) : base(serviceProvider)
        {
            _MfaService = MfaService;
        }

        /// <summary>
        /// Get client setup for Mfa
        /// This is a mix of organisation and user account Mfa
        /// </summary>
        /// <returns></returns>
        [HttpPost("SetupMfa")]
        public async Task<IActionResult> SetupMfa([FromBody] long id)
        {
            var key = await _MfaService.SetupMfa(id);

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

        [HttpPost("VerifyMfa")]
        public async Task<IActionResult> VerifyMfa([FromBody] MfaVerifyDto request)
        {
            var result = await _MfaService.VerifyMfaCode(request.Id, request.MfaCode);

            return Ok(new _ResponseDto
            {
                Valid = result
            });

        }



    }
}