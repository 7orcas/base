namespace BackendTest
{
    public class GlobalConstants
    {
        public const string ConnString            = "Host=localhost;Port=5432;Database=blue;Username=postgres;Password=js;";

        public const int orgNr                    = -99;
        public const int OrgNr                    = 204;
        public const string OrgLangCode           = "en";
        public const int UserId                   = -2;
        public const string UserName              = "TestUser1";
        public const string UserPW                = "xxx";
        public const string UserOrgs              = "1,2,204";
        public const string UserLangCode          = "en";
        public static readonly string[] Languages = { "en", "de", "c1", "c2" };

        
        
        public const string TOrg            = "base.org";
        public const string TUser           = "base.zzz";
        //public const string TPermission     = "base.permission"; DELETE ME
        public const string TRole           = "base.role";
        public const string TRolePermission = "base.rolepermission";
        public const string TUserRole       = "base.useraccrole";



    }
}
