using Microsoft.AspNetCore.DataProtection;

namespace Backend.Base.DataProtection
{
    public class MfaKeyProtector

    {
        private readonly IDataProtector _protector;

        public MfaKeyProtector(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("MFA-BLUE-KEY-V1");
        }

        public string Protect(string key)
        {
            return _protector.Protect(key);
        }

        public string Unprotect(string encryptedKey)
        {
            return _protector.Unprotect(encryptedKey);
        }
    }

}
