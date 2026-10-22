namespace FrontendServer
{
    public class GlobalConstants : Common.GlobalConstants
    {
        public const int AppClient                = 1; //defined in FrontendLogin as well
                
        public const string Date_Format_ddMMMyy   = "dd MMM yy";
        public const string Date_Format_ddMMMyyyy = "dd MMM yyyy";

        public const string VersionKey            = "kV";
        public const string MainMenuCacheKey      = "kMMenu";
        public const string LabelCacheKey         = "kLabel";
        public const string UrlCacheKey           = "kUrl";
        public const string PermCacheKey          = "kPerm";
        public const string TokenCacheKey         = "kToken";
        public const string RefreshTokenCacheKey  = "kRefreshToken";
        public const string ConfigCacheKey        = "kConf";
        public const string FieldConfigCacheKey   = "kFConf";
        public const string AuthorizedClientKey   = "kAC";
        public const string UnAuthorizedClientKey = "kUAC";
        public const string BearerKey             = "Bearer";

        /* DELETE ME
        private const string URL_base             = "api/";
        public const string URL_ulrs              = URL_base + "Urls/";

        private const string URL_user             = URL_base + "User/";
        private const string URL_audit            = URL_base + "Audit/";

        public const string URL_logout            = URL_base + "Logout/logout";
        public const string URL_perm_user         = URL_base + "Permission/userlist";
        public const string URL_perm_user_eff     = URL_base + "Permission/userlisteff";
        public const string URL_perm_list         = URL_base + "Permission/list";

        public const string URL_audit_search      = URL_audit + "search";
        public const string URL_audit_list        = URL_audit + "list";
        public const string URL_audit_get         = URL_audit + "get/";

        public const string URL_user_search       = URL_user + "search";
        public const string URL_user_list         = URL_user + "list";
        public const string URL_user_get          = URL_user + "get/";
        public const string URL_user_update       = URL_user + "update";
        public const string URL_user_new          = URL_user + "new";
        public const string URL_user_definition   = URL_user + "definition";

        public const string URL_user_role_list    = URL_base + "Role/userroles";
        public const string URL_role_list         = URL_base + "Role/roles";
        public const string URL_role              = URL_base + "Role/get/";
        public const string URL_role_update       = URL_base + "Role/update/";


        public const string URL_label_clientlist  = URL_base + "Label/clientlist/";
        public const string URL_label_relatedlist = URL_base + "Label/relatedlist/";
        public const string URL_config            = "api/Config/clientConfig";
        */

        public enum TextSize
        {
            None       = 0,
            Heading    = 1,
            SubHeading = 2,
            Section    = 3,
            Large      = 4,
            Normal     = 5,
            Small      = 6,
            ButtonText = 7,
        }

        public enum TextFieldWidth
        {
            Ignore = 0,
            TextVeryShort = 1,
            TextShort = 2,
            TextMedium = 3,
            TextLong = 4,
            Int = 5,
        }

    }
}
