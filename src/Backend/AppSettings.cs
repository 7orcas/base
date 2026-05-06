namespace Backend
{
    /// <summary>
    /// Application wide settings
    /// Read in on start up from appsettings.json
    /// </summary>
    public class AppSettings
    {
        public static string DBMainConnection { get; set; }
        public static int MaxGetTokenCalls { get; set; }
        public static int CacheExpirationAddSeconds { get; set; }
        public static int CacheExpirationGetSeconds { get; set; }
        public static string MainClientUrl { get; set; }
        public static string PathBase { get; set; }
        
        public static AppServiceAccount? ServiceAccount { get; set; }
    }

    public class AppServiceAccount 
    {
        public string UserId { get; set; }
        public string UserPw { get; set; }
        public string AttemptsFile { get; set; }
        
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(UserId) &&
                !string.IsNullOrEmpty(UserPw) &&
                !string.IsNullOrEmpty(AttemptsFile);
        }
    }

}
