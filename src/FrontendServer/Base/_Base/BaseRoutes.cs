using GC = FrontendServer.GlobalConstants;

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
        public const string UserRoute        = "user";

        //Page Codes
        public const string HomeCode         = "hme001";
        public const string ChangePWCode     = "cpw001";
        public const string OrgPageCode      = "org001";
        public const string ConfigPageCode   = "con001";
        public const string PermPageCode     = "per001";
        public const string UserRolePageCode = "uro001";
        public const string RolePageCode     = "rol001";
        public const string AuditPageCode    = "aud001";
        public const string UserPageCode     = "usr001";

        public const int PageCodeIndex = 0;
        public const int RouteIndex = 1;
        public const int PermissionIndex = 2;
        public const int MaxFields = 3; //PageCode, Route, Permission Nr
        static protected readonly object[] BasePageCodeRoutes = {
            HomeCode,               HomeRoute,        -1,
            ChangePWCode,           ChangePWRoute,    -1,
            OrgPageCode,            OrgRoute,         GC.PerOrg,
            ConfigPageCode,         ConfigRoute,      GC.PerConfig,
            PermPageCode,           PermRoute,        GC.PerPerm,
            UserRolePageCode,       UserRoleRoute,    -1,
            RolePageCode,           RoleRoute,        GC.PerRole,
            AuditPageCode,          AuditRoute,       GC.PerAudit,
            UserPageCode,           UserRoute,        GC.PerUser,
        };

    }
}
