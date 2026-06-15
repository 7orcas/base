using Backend.Base.Mfa.Ent;
using Microsoft.AspNetCore.Mvc;
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
            var login = await _loginService.GetLogin(id);

            if (login == null || login.MfaEnabled)
                return null;

            var key = GenerateMfaKey();
            var email = "js@7orcas.com";
            var qrCodeUri = GenerateQrCodeUri(email, key);

            return new MfaSetup
            {
                SharedKey = key,
                QrCodeUri = qrCodeUri
            };
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
