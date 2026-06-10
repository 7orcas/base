using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GC = Backend.GlobalConstants;

/// <summary>
/// Created: June 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Mfa
{
    [Authorize]
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
        /// Get client Mfa
        /// This is a mix of organisation and user account Mfa
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudIgnore)]

        [HttpPost("mfa/setup")]
        public async Task<IActionResult> SetupMfa()
        {
            var user = await _userManager.GetUserAsync(User);

            var key = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(key))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                key = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            var email = user.Email;

            var uri = $"otpauth://totp/YourApp:{email}?secret={key}&issuer=YourApp&digits=6";

            return Ok(new
            {
                sharedKey = key,
                qrCodeUri = uri
            });
        }



        [HttpPost("mfa/verify")]
        public async Task<IActionResult> VerifyMfa([FromBody] VerifyMfaRequest request)
        {
            var user = await _userManager.GetUserAsync(User);

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                request.Code);

            if (!isValid)
                return BadRequest("Invalid code");

            await _userManager.SetTwoFactorEnabledAsync(user, true);

            return Ok();
        }



    }
}