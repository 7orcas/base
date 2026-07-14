using GC = FrontendLogin.GlobalConstants;

/// <summary>
/// Remember Me methods for Login
/// Created: June 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace FrontendLogin.Pages
{
    public partial class Login
    {

        private string GetRememberMeCookie(string cookie)
        {
            try
            {
                HttpContextAccessor.HttpContext.Request.Cookies.TryGetValue(cookie, out var value);
                return value;
            }
            catch
            {
                return null;
            }
        }

        private void parseCookie(string cookie)
        {
            if (string.IsNullOrWhiteSpace(cookie))
                return;

            loginRequest.RememberMe = true; // if cookie exists, it means RememberMe was checked
            var parts = cookie.Split(',');

            foreach (var part in parts)
            {
                var kv = part.Split('=', 2); // split only once
                if (kv.Length != 2) continue;

                var key = kv[0].Trim();
                var value = kv[1].Trim();

                switch (key)
                {
                    case "u":
                        loginRequest.UserName = value;
                        break;
                    case "p":
                        loginRequest.Password = value;
                        break;
                    case "o":
                        loginRequest.Org = int.Parse(value);
                        break;
                    case "l":
                        loginRequest.LangCode = value;
                        break;
                }
            }
        }
    }
}
