using GC = Backend.GlobalConstants;
using GCT = BackendTest.GlobalConstants;

namespace BackendTest.Setup
{
    public partial class BaseTest
    {
        private static int idTest = -1;
        public static int MaxRoles = 10;
        public static int MaxPermissions = 10;

        public static async Task<bool> SetupTestDb()
        {
            try
            {
                AppSettings.DBMainConnection = GCT.ConnString;
                //await DeleteAll();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        protected static async Task InsertOrg()
        {
            await Sql.Execute(
                "INSERT INTO " + GCT.TOrg + " " +
                    "(nr, code, descr, langCode, langlabelvariant, encoded) " +
                "VALUES (" +
                    GCT.OrgNr +
                    ",'Test Org'" +
                    ",'Test Org Descr'" +
                    ",'" + GCT.OrgLangCode + "'" +
                    ",1" +
                    ",'{Languages:[{LangCode:\"en\",IsEditable:true},{LangCode:\"de\",IsEditable:true},{LangCode:\"c1\",IsEditable:false},{LangCode:\"c2\",IsEditable:false}]}'" +
                    ");" 
                );
        }
        protected static async Task InsertUser()
        {
            await Sql.Execute(
                "INSERT INTO " + GCT.TUser + " " +
                    "(id, xxx, yyy)" + // orgs, langCode) " +
                "VALUES (" +
                    GCT.UserId +
                    ",'" + GCT.UserName + "'" +
                    ",'" + GCT.UserPW + "'" +
                    ");"
                );
        }

        protected static async Task InsertUserAcc()
        {
            await Sql.Execute(
                "INSERT INTO " + GCT.TUserAccount + " " +
                    "(id, zzzid, orgnr, langcode, isadmin)" + 
                "VALUES (" +
                    GCT.UserAccountId +
                    "," + GCT.UserId + 
                    "," + GCT.OrgNr +
                    ",'" + GCT.OrgLangCode + "'" +
                    ",false" +
                    ");"
                );
        }

        protected static async Task InsertUserAccRole()
        {
            await Sql.Execute(
                "INSERT INTO " + GCT.TUserAccountRole + " " +
                    "(id, useraccid, roleid)" +
                "VALUES (" +
                    GCT.UserAccountRoleId +
                    "," + GCT.UserAccountId +
                    "," + GCT.RoleId +
                    ");"
                );
        }

        protected static async Task InsertRole()
        {
            await Sql.Execute(
                "INSERT INTO " + GCT.TRole + " " +
                    "(id, orgnr, code)" +
                "VALUES (" +
                    GCT.RoleId +
                    ",0" +
                    ",'test'" +
                    ");"
                );
        }

        protected static async Task InsertRolePermission()
        {
            await Sql.Execute(
                "INSERT INTO " + GCT.TRolePermission + " " +
                    "(id, roleid, permissionnr, crud)" +
                "VALUES (" +
                    GCT.RolePermissionId +
                    "," + GCT.RoleId +
                    "," + GC.PerPerm7 +
                    ",'crud'" +
                    ");"
                );
        }

        protected static async Task InsertUserPermissionRole(string tUR, string tP, string tR, string tRP)
        {
            PermissionEnt[] perms = new PermissionEnt[MaxPermissions];
            for (int i = 0; i < MaxPermissions; i++)
                perms[i] = new PermissionEnt { Nr = -1 + i*-1, LangKey = "perm" + (i+1) };

            string sql = "";
            
            //DELETE ME
            //foreach (var rec in perms)
            //    sql += "INSERT INTO " + tP + " (id, code) VALUES (" + rec.Nr + ",'" + rec.LangKey + "');";
            //await Sql.Execute(IdentityInsert(tP, sql));


            RoleEnt[] roles = new RoleEnt[MaxRoles];
            for (int i = 0; i< MaxRoles; i++)
                roles[i] = new RoleEnt { Id = -1 + i*-1, Code = "role" + (i + 1), OrgNr = GCT.OrgNr };

            sql = "";
            foreach (var rec in roles)
                sql += "INSERT INTO " + tR + " (id, code, orgNr) VALUES (" + rec.Id + ",'" + rec.Code + "'," + rec.OrgNr + ");";
            await Sql.Execute(sql);


            string[] crud = { "c","r","u","d", "crud", "xyz", "crudcrudz", "ddddd" };
            int p = 0;
            sql = "";
            foreach (var rec1 in roles)
            {
                if (p < crud.Length - 1) p++;
                else p = 0;
                string c = crud[p];
                foreach (var rec2 in perms)
                    sql += "INSERT INTO " + tRP + " (id, roleId, permissionnr, crud) VALUES (" + --idTest + "," + rec1.Id + "," + rec2.Nr + ",'" + c + "');";
            }
            await Sql.Execute(sql);

            sql = "";
            foreach (var rec in roles)
                sql += "INSERT INTO " + tUR + " (id, zzzId, roleId) VALUES (" + --idTest + "," +  GCT.UserId + "," + rec.Id + ");";
            await Sql.Execute(sql);
        }

        protected static async Task DeleteAll()
        {
            string[] tables = {
                GCT.TRolePermission
                ,GCT.TUserAccountRole
                ,GCT.TUserAccount
                ,GCT.TRole
                ,GCT.TUser
            };
            string[] tablesX = {
                GCT.TOrg
            };

            foreach (var table in tables)
                await Sql.Execute("Delete from " + table + " WHERE id < 0");

            foreach (var table in tablesX)
                await Sql.Execute("Delete from " + table + " WHERE nr < 0");

        }

    }
}
