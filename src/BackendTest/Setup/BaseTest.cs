using Microsoft.Extensions.Caching.Memory;
using GC = Backend.GlobalConstants;
using GCT = BackendTest.GlobalConstants;

namespace BackendTest.Setup
{
    public partial class BaseTest
    {
        public const bool RunCleanup = false;
        public const string ConnString = "Host=localhost;Port=5432;Database=blue;Username=postgres;Password=js;";

        public const int OrgNr = -99;
        public const string OrgLangCode = "en";
        public const int UserId = -2;
        public const int UserAccountId = -3;
        public const int UserAccountRoleId = -4;
        public const int RoleId = -5;
        public const int RolePermissionId = -7;
        public const string UserName = "TestUser1";
        public const string UserPW = "xxx";
        public const string UserLangCode = "en";
        public static readonly string[] Languages = { "en", "de", "c1", "c2" };

        public const string TOrg = "base.org";
        public const string TUser = "base.zzz";
        public const string TUserAccount = "base.useracc";
        public const string TUserAccountRole = "base.useraccrole";
        public const string TRole = "base.role";
        public const string TRolePermission = "base.rolepermission";

        public const int IdStartRange         = 1000;
        public const int IdStartLabel         = 1000001;
        public const int IdStartPermission    = 1001001;
        public const int IdStartConfig        = 1002001;
        public const int IdStartConfigInitial = 1003001;

        public IMemoryCache memoryCache;

        public BaseTest() 
        {
            memoryCache = new MemoryCache(new MemoryCacheOptions());
        }
               

    }
}
