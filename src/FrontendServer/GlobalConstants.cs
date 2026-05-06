namespace FrontendServer
{
    public class GlobalConstants : Common.GlobalConstants
    {
        public const int AppClient                = 1; //defined in FrontendLogin as well
        
        public const string LabelCacheKey         = "kLabel";
        public const string TokenCacheKey         = "kToken";
        public const string ConfigCacheKey        = "kConfig";
        public const string AuthorizedClientKey   = "kAC";
        public const string UnAuthorizedClientKey = "kUAC";
        public const string BearerKey             = "Bearer";

        public const string URL_token             = "api/Token/token";
        public const string URL_logout            = "api/Logout/logout";
        public const string URL_perm_user         = "api/Permission/userlist";
        public const string URL_perm_user_eff     = "api/Permission/userlisteff";
        public const string URL_perm_list         = "api/Permission/list";
        public const string URL_audit_list        = "api/Audit/list";
        public const string URL_user_role_list    = "api/Role/userroles";
        public const string URL_role_list         = "api/Role/roles";
        public const string URL_role              = "api/Role/get/";
        public const string URL_role_update       = "api/Role/update/";
        public const string URL_config            = "api/Config/clientConfig";
        public const string URL_org_list          = "api/Org/list";
        public const string URL_org               = "api/Org/get/";
        public const string URL_org_update        = "api/Org/update";
        public const string URL_label_clientlist  = "api/Label/clientlist/";
        public const string URL_label_relatedlist = "api/Label/relatedlist/";


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
            Short = 1,
            Medium = 2,
            Long = 3,
        }

    }
}
