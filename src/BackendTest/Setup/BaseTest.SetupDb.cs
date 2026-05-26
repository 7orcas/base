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
                AppSettings.DBMainConnection = ConnString;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        protected static async Task InsertOrg(int idStart)
        {
            await Sql.Execute(
                "INSERT INTO " + TOrg + " " +
                    "(nr, code, descr, langCode, langlabelvariant, encoded) " +
                "VALUES (" +
                    idStart +
                    ",'Test Org'" +
                    ",'Test Org Descr'" +
                    ",'" + OrgLangCode + "'" +
                    ",1" +
                    ",'{Languages:[{LangCode:\"en\",IsEditable:true},{LangCode:\"de\",IsEditable:true},{LangCode:\"c1\",IsEditable:false},{LangCode:\"c2\",IsEditable:false}]}'" +
                    ");" 
                );
        }
        protected static async Task InsertUser(int idStart)
        {
            await Sql.Execute(
                "INSERT INTO " + TUser + " " +
                    "(id, xxx, yyy)" + // orgs, langCode) " +
                "VALUES (" +
                    idStart +
                    ",'" + (UserName + "_" + idStart ) + "'" +
                    ",'" + UserPW + "'" +
                    ");"
                );
        }

        protected static async Task InsertUserAcc(int idStart)
        {
            await Sql.Execute(
                "INSERT INTO " + TUserAccount + " " +
                    "(id, zzzid, orgnr, langcode, isadmin)" + 
                "VALUES (" +
                    idStart +
                    "," + idStart + 
                    "," + idStart +
                    ",'" + OrgLangCode + "'" +
                    ",false" +
                    ");"
                );
        }

        protected static async Task InsertUserAccRole(int idStart)
        {
            await Sql.Execute(
                "INSERT INTO " + TUserAccountRole + " " +
                    "(id, useraccid, roleid)" +
                "VALUES (" +
                    idStart +
                    "," + idStart +
                    "," + idStart +
                    ");"
                );
        }

        protected static async Task InsertRole(int idStart)
        {
            await Sql.Execute(
                "INSERT INTO " + TRole + " " +
                    "(id, orgnr, code)" +
                "VALUES (" +
                    idStart +
                    ",0" +
                    ",'" + ("ServiceTest_" + idStart) + "'" +
                    ");"
                );
        }

        protected static async Task InsertRolePermission(int idStart)
        {
            await Sql.Execute(
                "INSERT INTO " + TRolePermission + " " +
                    "(id, roleid, permissionnr, crud)" +
                "VALUES (" +
                    idStart +
                    "," + idStart +
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
                roles[i] = new RoleEnt { Id = -1 + i*-1, Code = "role" + (i + 1), OrgNr = OrgNr };

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
                sql += "INSERT INTO " + tUR + " (id, zzzId, roleId) VALUES (" + --idTest + "," +  UserId + "," + rec.Id + ");";
            await Sql.Execute(sql);
        }

        protected static async Task DeleteAll(int idStart)
        {
            string[] tables = {
                TRolePermission
                ,TUserAccountRole
                ,TUserAccount
                ,TRole
                ,TUser
            };
            string[] tablesX = {
                TOrg
            };

            foreach (var table in tables)
                await Sql.Execute("Delete from " + table + " WHERE id >= " + idStart + " AND id < " + (idStart + IdStartRange));

            foreach (var table in tablesX)
                await Sql.Execute("Delete from " + table + " WHERE nr >= " + idStart + " AND nr < " + (idStart + IdStartRange));

        }

    }
}
