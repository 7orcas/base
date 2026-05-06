namespace Backend
{
    public class GlobalConstants : Common.GlobalConstants
    {
        public const int BaseOrgNr   = 0;
                
        public const int AppClient          = 1; //defined in FrontendLogin as well
        public const string LangCodeDefault = "en";

        public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        //IMemoryCache keys
        public const string CacheKeyTokenPrefix     = "TS_";
        public const string CacheKeyOrgPrefix       = "OS_";
        public const string CacheKeyOrgConfigPrefix = "CS_";
        public const string CacheKeySessionPrefix   = "SS_";
        public const string CacheKeyLabelPrefix     = "LK_";
        public const string CacheKeyPermDic         = "PS_dic";

        //Entity Type Id
        public const int EntityTypePermUser         = 10;
        public const int EntityTypePermUserEffect   = 11;
        public const int EntityTypePermList         = 12;
        public const int EntityTypeRole             = 30;
        public const int EntityTypeAudit            = 40;
        public const int EntityTypeLogin            = 50;
        public const int EntityTypeLogout           = 60;
        public const int EntityTypeLangCode         = 70;
        public const int EntityTypeLangKey          = 80;
        public const int EntityTypeLangLabelList    = 90;
        public const int EntityTypeLangLabelRelated = 100;
        public const int EntityTypeConfig           = 110;
        public const int EntityTypeOrg              = 120;

        public const int EntityTypeMachine          = 10001;

        public static readonly object[] EntityTypes = {
            EntityTypePermUser,         "PermUser",
            EntityTypePermUserEffect,   "PermUserEff",
            EntityTypePermList,         "PermList",
            EntityTypeRole,             "Role",
            EntityTypeAudit,            "Audit",
            EntityTypeLogin,            "Login",
            EntityTypeLogout,           "Logout",
            EntityTypeLangCode,         "LangCode",
            EntityTypeLangKey,          "LangKey",
            EntityTypeLangLabelList,    "LangLabelList",
            EntityTypeLangLabelRelated, "LangLabelRelated",
            EntityTypeConfig,           "Config",
            EntityTypeOrg,              "Org",

            EntityTypeMachine,          "Machine",
        };

        //Service
        public const long ServiceLoginId             = -1L;
        public const long ServiceAccountId           = -1L;
        public const string ServiceAccountName       = "Service";

    }
}
