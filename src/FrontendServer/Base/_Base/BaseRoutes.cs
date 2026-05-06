namespace FrontendServer.Base._Base
{
    public class BaseRoutes
    {
        //Routes
        public const string HomeRoute        = "/";
        public const string LogoutRoute      = "logout";
        public const string ChangePWRoute    = "changePW";
        public const string OrgRoute         = "organisation";
        public const string ConfigRoute      = "config";
        public const string PermRoute        = "permission";
        public const string UserRoleRoute    = "userrole";
        public const string RoleRoute        = "role";
        public const string AuditRoute       = "audit";

        //Page Codes
        public const string HomeCode         = "hme001";
        public const string ChangePWCode     = "cpw001";
        public const string OrgPageCode      = "org001";
        public const string ConfigPageCode   = "con001";
        public const string PermPageCode     = "per001";
        public const string UserRolePageCode = "uro001";
        public const string RolePageCode     = "rol001";
        public const string AuditPageCode    = "aud001";

        static protected readonly string[] BasePageCodeRoutes = {
            HomeCode,               HomeRoute,
            ChangePWCode,           ChangePWRoute,
            OrgPageCode,            OrgRoute,
            ConfigPageCode,         ConfigRoute,
            PermPageCode,           PermRoute,
            UserRolePageCode,       UserRoleRoute,
            RolePageCode,           RoleRoute,
            AuditPageCode,          AuditRoute,
        };

    }
}
