using Backend.Base.Mfa.Ent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using OtpNet;
using GC = Backend.GlobalConstants;

/// <summary>
/// Created: 
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Mfa
{
    public class MfaService : BaseService, MfaServiceI
    {
        private readonly LoginServiceI _loginService;

        public MfaService(IServiceProvider serviceProvider,
            LoginServiceI loginService) 
            : base(serviceProvider)
        {
            _loginService = loginService;
        }


        public async Task<MfaSetup?> SetupMfa(long id)
        {
            var login = await _loginService.GetLoginById(id);

            if (login == null || login.MfaEnabled || !string.IsNullOrEmpty(login.MfaSecret))
                return null;

            var key = GenerateMfaKey();
            var qrCodeUri = GenerateQrCodeUri(login.Email, key);
            _loginService.SetMfaKey(id, key);


            return new MfaSetup
            {
                SharedKey = key,
                QrCodeUri = qrCodeUri
            };
        }

        public async Task<bool> VerifyMfaCode(long id, string mfaCode)
        {
            var login = await _loginService.GetLoginById(id);

            if (login == null || login.MfaSecret == null)
                return false;

            var totp = new Totp(Base32Encoding.ToBytes(login.MfaSecret));

            var result = totp.VerifyTotp(
                mfaCode.Trim(),
                out _,
                new VerificationWindow(2, 2) // allows slight clock drift
            );

            if (result && !login.MfaEnabled)
                _loginService.EnableMfa(id);

            return result;
        }


        private string GenerateMfaKey()
        {
            var key = KeyGeneration.GenerateRandomKey(20); // 20 bytes secure random
            return Base32Encoding.ToString(key);
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            var name = AppSettings.AuthenticatorAppName;
            return $"otpauth://totp/{name}:{email}?secret={unformattedKey}&issuer={name}&digits=6";
        }

    }

}
