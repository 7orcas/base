using Microsoft.AspNetCore.DataProtection;

namespace Backend.Base.DataProtection
{
    public class CookieProtector
    {
        private readonly IDataProtector _protector;

        public CookieProtector(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("RememberMeCookieProtector");
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;
            return _protector.Protect(plainText);
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return null;
            return _protector.Unprotect(cipherText);
        }
    }
}
