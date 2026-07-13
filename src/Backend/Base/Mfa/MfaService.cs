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
        private readonly LoginRepoI _loginRepo;

        public MfaService(IServiceProvider serviceProvider,
            LoginRepoI loginRepo) 
            : base(serviceProvider)
        {
            _loginRepo = loginRepo;
        }


        public async Task<MfaSetup?> SetupMfa(long id, string username)
        {
            var login = await _loginRepo.GetLoginById(id);

            if (login == null || login.IsMfaEnabled || !string.IsNullOrEmpty(login.MfaSecret))
                return null;

            var isService = id == GC.ServiceLoginId && username == GC.ServiceUsername;
            var key = GenerateMfaKey();
            var qrCodeUri = GenerateQrCodeUri(login.Email, key, isService);
            _loginRepo.SetMfaKey(id, key);


            return new MfaSetup
            {
                SharedKey = key,
                QrCodeUri = qrCodeUri
            };
        }

        public async Task<bool> VerifyMfaCode(long id, string mfaCode)
        {
            var login = await _loginRepo.GetLoginById(id);

            if (login == null || login.MfaSecret == null)
                return false;

            var totp = new Totp(Base32Encoding.ToBytes(login.MfaSecret));

            var result = totp.VerifyTotp(
                mfaCode.Trim(),
                out _,
                new VerificationWindow(2, 2) // allows slight clock drift
            );

            if (result && !login.IsMfaEnabled)
                _loginRepo.EnableMfa(id);

            return result;
        }


        private string GenerateMfaKey()
        {
            var key = KeyGeneration.GenerateRandomKey(20); // 20 bytes secure random
            return Base32Encoding.ToString(key);
        }

        private string GenerateQrCodeUri(string email, string unformattedKey, bool isService)
        {
            var name = AppSettings.AuthenticatorAppName + (isService?"-Service":"");
            return $"otpauth://totp/{name}:{email}?secret={unformattedKey}&issuer={name}&digits=6";
        }

    }

}
