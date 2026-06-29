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
        public static int AccessTokenMinutes { get; set; }
        public static int RefreshTokenDays { get; set; }
        public static int CacheExpirationAddSeconds { get; set; }
        public static int CacheExpirationGetSeconds { get; set; }
        public static string CorsAllowedOrigins { get; set; }
        public static string PathBase { get; set; }
        public static string AuthenticatorAppName { get; set; }
        public static AppServiceAccount? ServiceAccount { get; set; }
        public static EmailSettings? EmailSettings { get; set; }
        public static AppUrls Urls { get; set; }
    }

    public class AppServiceAccount 
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string UserPw { get; set; }
        public string AttemptsFile { get; set; }
        
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(UserId) &&
                !string.IsNullOrEmpty(UserPw) &&
                !string.IsNullOrEmpty(AttemptsFile);
        }
    }

    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

}
