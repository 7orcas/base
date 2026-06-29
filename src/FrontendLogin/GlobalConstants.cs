namespace FrontendLogin
{
    public class GlobalConstants : Common.GlobalConstants
    {
        public const string HTTP_Client             = "BackendApi";
        public const string URL_login               = "api/Login/login";
        public const string URL_login_org           = "api/Login/org";
        public const string URL_reset_action        = "api/Login/resetaction";
        public const string URL_show_password_rules = "api/Login/passwordrules";
        public const string URL_mfa_verify          = "api/Mfa/verifyMfa";
        public const string URL_mfa_setup           = "api/Mfa/setupMfa";
    }
}
