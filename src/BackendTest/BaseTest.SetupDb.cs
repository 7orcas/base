using GC = Backend.GlobalConstants;
using GCT = BackendTest.GlobalConstants;

namespace BackendTest
{
    public partial class BaseTest
    {
        private static int idTest = -1;
        public static int MaxRoles = 10;
        public static int MaxPermissions = 10;

        public static async Task<bool> SetupTestDb()
        {
            initialisedDb = false;

            try
            {
                AppSettings.DBMainConnection = GCT.ConnString;
                await DeleteAll();
                await InsertOrg(GCT.TOrg);
                await InsertUser(GCT.TUser);
                await InsertUserPermissionRole(GCT.TUserRole, GCT.TPermission, GCT.TRole, GCT.TRolePermission);
                initialisedDb = true;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private static async Task InsertOrg(string t)
        {
            await Sql.Execute(
                IdentityInsert(t,
                    "INSERT INTO " + t + " " +
                        "(id, nr, code, descr, langCode, encoded) " +
                    "VALUES (" +
                        GCT.orgNr +
                        "," + GCT.OrgNr +
                        ",'Test Org'" +
                        ",'Test Org Descr'" +
                        ",'" + GCT.OrgLangCode + "'" +
                        ",'{IsLangCodeEditable:false,Languages:[" + string.Join(",", GCT.Languages.Select(lang => $"\"{lang}\"")) + "]}'" +
                        ");" 
                    ));
        }

        private static async Task InsertUser(string t)
        {
            await Sql.Execute(
                IdentityInsert(t,
                    "INSERT INTO " + t + " " +
                        "(id, xxx, yyy, orgs, langCode) " +
                    "VALUES (" +
                        GCT.UserId +
                        ",'" + GCT.UserName + "'" +
                        ",'" + GCT.UserPW + "'" +
                        ",'" + GCT.UserOrgs + "'" +
                        ",'" + GCT.UserLangCode + "'" +
                        ");"
                    ));
        }

        private static async Task InsertUserPermissionRole(string tUR, string tP, string tR, string tRP)
        {
            PermissionEnt[] perms = new PermissionEnt[MaxPermissions];
            for (int i = 0; i < MaxPermissions; i++)
                perms[i] = new PermissionEnt { Nr = -1 + i*-1, LangKey = "perm" + (i+1) };

            string sql = "";
            foreach (var rec in perms)
                sql += "INSERT INTO " + tP + " (id, code) VALUES (" + rec.Nr + ",'" + rec.LangKey + "');";
            await Sql.Execute(IdentityInsert(tP, sql));


            RoleEnt[] roles = new RoleEnt[MaxRoles];
            for (int i = 0; i< MaxRoles; i++)
                roles[i] = new RoleEnt { Id = -1 + i*-1, Code = "role" + (i + 1), OrgNr = GCT.orgNr };

            sql = "";
            foreach (var rec in roles)
                sql += "INSERT INTO " + tR + " (id, code, orgNr) VALUES (" + rec.Id + ",'" + rec.Code + "'," + rec.OrgNr + ");";
            await Sql.Execute(IdentityInsert(tR, sql));


            string[] crud = { "c","r","u","d", "crud", "xyz", "crudcrudz", "ddddd" };
            int p = 0;
            sql = "";
            foreach (var rec1 in roles)
            {
                if (p < crud.Length - 1) p++;
                else p = 0;
                string c = crud[p];
                foreach (var rec2 in perms)
                    sql += "INSERT INTO " + tRP + " (id, roleId, permissionId, crud) VALUES (" + --idTest + "," + rec1.Id + "," + rec2.Nr + ",'" + c + "');";
            }
            await Sql.Execute(IdentityInsert(tRP, sql));

            sql = "";
            foreach (var rec in roles)
                sql += "INSERT INTO " + tUR + " (id, zzzId, roleId) VALUES (" + --idTest + "," +  GCT.UserId + "," + rec.Id + ");";
            await Sql.Execute(IdentityInsert(tUR, sql));
        }


        private static string IdentityInsert(string table, string sql)
        {
            if (!sql.EndsWith(";"))
                sql += ";";

            return "SET IDENTITY_INSERT " + table + " ON;" +
                sql +
                "SET IDENTITY_INSERT " + table + " OFF;";
        }

        private static async Task DeleteAll()
        {
            string[] tables = {
                GCT.TRolePermission
                ,GCT.TPermission
                ,GCT.TUserRole
                ,GCT.TRole
                ,GCT.TUser
                ,GCT.TOrg
            };

            foreach (var table in tables)
                await Sql.Execute("Delete from " + table + " WHERE id < 0");
        }

    }
}
