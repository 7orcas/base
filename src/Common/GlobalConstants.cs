
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
        public const int NewRecordId = -9000;

        public const int StatusCodeOk            = 200;
        public const int StatusCodeNotAuthorised = 403;
        public const int StatusCodeUnProcessable = 422;

        //Labels
        public const string LabelParameterPrefix = "%%";

        //Field Lengths
        public const int LenCode        = 15;
        public const int LenDescription = 50;
        public const int LenLangCode    = 4;

        //Validation Status
        public const int ValStatusOk      = 0;
        public const int ValStatusError   = 1;
        
        //Permissions
        public const string CrudCreate   = "c";
        public const string CrudRead     = "r";
        public const string CrudUpdate   = "u";
        public const string CrudDelete   = "d";
        public const string CrudReadList = "l";
        public const string CrudIgnore   = "x"; //Don't need a permission crud setting

        public const int PerIgnore  = 0;
        public const int PerOrg     = 1;
        public const int PerPerm    = 2;
        public const int PerLabel   = 3;
        public const int PerLang    = 4;
        public const int PerRole    = 5;
        public const int PerUser    = 6;
        public const int PerAudit   = 7;
        public const int PerConfig  = 8;

        public const int PerMach    = 101;
        public const int PerPerm1   = 102;
        public const int PerPerm2 = 103;
        public const int PerPerm3 = 104;
        public const int PerPerm4 = 105;
        public const int PerPerm5 = 106;
        public const int PerPerm6 = 107;
        public const int PerPerm7 = 108;
        public const int PerPerm8 = 109;
        public const int PerPerm9 = 110;
        public const int PerPerm10 = 111;
        public const int PerPerm11 = 112;
        public const int PerPerm12 = 113;
        public const int PerPerm13 = 114;
        public const int PerPerm14 = 115;
        public const int PerPerm15 = 116;
        public const int PerPerm16 = 117;
        public const int PerPerm17 = 118;
        public const int PerPerm18 = 119;
        public const int PerPerm19 = 120;
        public const int PerPerm20 = 121;

        public static readonly object[] Permissions = {
            PerOrg,      "Org",
            PerPerm,     "Perm", 
            PerLabel,    "Label",
            PerLang,     "Lang",
            PerRole,     "Role",
            PerUser,     "User",
            PerAudit,    "Audit",
            PerConfig,   "Conf",

            PerMach,     "Machine",

            PerPerm1  ,  "102",
            PerPerm2  ,  "103",
            PerPerm3  ,  "104",
            PerPerm4  ,  "105",
            PerPerm5  ,  "106",
            PerPerm6  ,  "107",
            PerPerm7  ,  "108",
            PerPerm8  ,  "109",
            PerPerm9  ,  "110",
            PerPerm10  ,"111",
            PerPerm11  ,"112",
            PerPerm12  ,"113",
            PerPerm13  ,"114",
            PerPerm14  ,"115",
            PerPerm15  ,"116",
            PerPerm16  ,"117",
            PerPerm17  ,"118",
            PerPerm18  ,"119",
            PerPerm19  ,"120",
            PerPerm20  ,"121",

        };

    }
}
