
/// <summary>
/// Common constants between all projects
/// Created: June 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Common
{
    public class GlobalConstants
    {
        public const int WebClient = 1;
        public const int ApiClient = 2;

        public const int NewRecordId = -9000;

        public const string LangCodeDefault      = "en";
        public const int OrgNrDefault            = 0;

        public const string Date_TS_Format = "yyyy-MM-dd HH:mm:ss";

        //Login (successful) Actions
        public const int NavigateToFrontendServer     = 0;
        public const int GetAndDisplayJWTtoken        = 1;
        public const string URL_version_number        = "api/version";
        public const string URL_login_option          = "api/LoginOption/get/";
        public const string URL_login_label           = "api/Login/loginlabels/";
        public const string URL_reset_request         = "api/Login/resetrequest";
        public const string URL_reset_action          = "api/Login/resetaction";
        public const string URL_token_token           = "api/Token/token";
        public const string URL_token_refresh_expired = "api/Token/refreshexpired";
        public const string URL_token_refresh_current = "api/Token/refreshcurrent";
        public const string URL_token_test            = "api/Token/test";
        
        public const string Cookie_RememberMe         = "remember_me_";

        //Service account
        public const string ServiceLoginUrlSuffix = "servicex";

        //Labels
        public const string LabelParameterPrefix = "%%";

        //Field Lengths
        public const int LenCode        = 15;
        public const int LenDescription = 50;
        public const int LenLangCode    = 4;
        public const int LenEmail       = 50;

        //Searches
        public enum TextSearchCompare
        {
            Start = 0,
            Contains = 1,
            Exact = 2
        }

        public enum TextSearchDelimiter
        {
            None = 0,
            Or = 1
        }

        //Validation Status
        public const int ValStatusOk      = 0;
        public const int ValStatusError   = 1;


        //Permissions and Audit Activities
        public const string CrudCreate   = "c";
        public const string CrudRead     = "r";
        public const string CrudUpdate   = "u";
        public const string CrudDelete   = "d";
        public const string CrudReadList = "l";
        public const string CrudAll = "crudl";
        public const string CrudAdminOnly = CrudCreate + CrudUpdate + CrudDelete; //Non admin users can't create, update and delete (if permission is admin-only)
        public const string AuditLogin = "i";
        public const string AuditLogout = "o";
        public const string CrudIgnore   = "x"; //Don't need a permission crud setting


        public const int PerIgnore    = 0;
        public const int PerOrg       = 1;
        public const int PerPerm      = 2;
        public const int PerLabel     = 3;
        //public const int PerLang      = 4; Reuse
        public const int PerRole      = 5;
        public const int PerUser      = 6;
        public const int PerAudit     = 7;
        public const int PerConfig    = 8;
        public const int PerReg       = 9;
        

        public const int PerMachine   = 101;
        public const int PerPerm1     = 102;
        public const int PerPerm2     = 103;
        public const int PerPerm3     = 104;
        public const int PerPerm4     = 105;
        public const int PerPerm5     = 106;
        public const int PerPerm6     = 107;
        public const int PerPerm7     = 108;
        public const int PerPerm8     = 109;
        public const int PerPerm9     = 110;
        public const int PerPerm10    = 111;
        public const int PerPerm11    = 112;
        public const int PerPerm12    = 113;
        public const int PerPerm13    = 114;
        public const int PerPerm14    = 115;
        public const int PerPerm15    = 116;
        public const int PerPerm16    = 117;
        public const int PerPerm17    = 118;
        public const int PerPerm18    = 119;
        public const int PerPerm19    = 120;
        public const int PerPerm20    = 121;

        public const int PermissionNrIndex = 0;
        public const int PermissionLabelIndex = 1;
        public const int PermissionAdminIndex = 2;
        public const int PermissionFields = 3;
        public static readonly object[] Permissions = { //Permission nr, LangKey, IsAdmin (ie only admin's can update this permission)
            PerOrg,      "Org",    true,
            PerPerm,     "Perm",   false,
            PerLabel,    "Label",  true,
            PerRole,     "Role",   false,
            PerUser,     "User",   true,
            PerAudit,    "Audit",  false,
            PerConfig,   "Conf",   false,

            PerMachine,  "Machine",false,

            PerPerm1  ,  "102",    false,
            PerPerm2  ,  "103",    false,
            PerPerm3  ,  "104",    false,
            PerPerm4  ,  "105",    false,
            PerPerm5  ,  "106",    false,
            PerPerm6  ,  "107",    false,
            PerPerm7  ,  "108",    false,
            PerPerm8  ,  "109",    false,
            PerPerm9  ,  "110",    false,
            PerPerm10  ,"111",     false,
            PerPerm11  ,"112",     false,
            PerPerm12  ,"113",     false,
            PerPerm13  ,"114",     false,
            PerPerm14  ,"115",     false,
            PerPerm15  ,"116",     false,
            PerPerm16  ,"117",     false,
            PerPerm17  ,"118",     false,
            PerPerm18  ,"119",     false,
            PerPerm19  ,"120",     false,
            PerPerm20  ,"121",     false,

        };

    }
}
