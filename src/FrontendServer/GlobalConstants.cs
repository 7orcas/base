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
        public const string LoginParamsCacheKey   = "kLParms";
        public const string FieldConfigCacheKey   = "kFConf";
        public const string AuthorizedClientKey   = "kAC";
        public const string UnAuthorizedClientKey = "kUAC";
        public const string BearerKey             = "Bearer";
                

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
