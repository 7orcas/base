using Npgsql;
using Microsoft.Extensions.Caching.Memory;
using GC = Backend.GlobalConstants;

namespace Backend.Base.Permission
{
    /// <summary>
    /// Manage loading and processing permissions for logged user
    /// Tables and relationships:
    ///  - base.role: roles, contains BaseorgNr general roles and orgNr specific roles
    ///  - hardcoded permissions in controllers (not orgNr related)
    ///  - base.rolePermission: relation between role and permission with allowed crud value
    ///  - base.zzz: user login with allowed orgNrs
    ///  - base.zzzRole: relation bewteen zzz (user) and role (BaseorgNr and orgNr user is logging into)
    /// </summary>
    /// <author>John Stewart</author>
    /// <created>March 3, 2025</created>
    /// <license>**Licence**</license>
    public class PermissionService: BaseService, PermissionServiceI
    {
        private readonly IMemoryCache _memoryCache;
        
        public PermissionService(IServiceProvider serviceProvider,
            IMemoryCache memoryCache) 
            : base(serviceProvider)
        { 
            _memoryCache = memoryCache;
        }


        public PermissionEnt GetPermissionEnt (int permNr)
        {
            if (_memoryCache.TryGetValue(GC.CacheKeyPermDic, out var cachedValue))
            {
                var dic = cachedValue as Dictionary<int, PermissionEnt>;
                if (dic != null && dic.ContainsKey(permNr)) 
                    return dic[permNr];
            }
            return null;
        }

        /// <summary>
        /// Load and setup a user's permission/crud settings
        /// The specifig org's permissions are used if exist, then org 0 permission's are the default
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task<List<PermissionCrudEnt>> LoadEffectivePermissions(SessionEnt session)
        {
            return await LoadEffectivePermissionsInt(session.UserAccount.Id, session.Org.Nr);
        }


        /// <summary>
        /// Load and setup a user's permission/crud settings
        /// The specific org's permissions are used if exist, then org 0 permission's are the default
        /// </summary>
        /// <param name="userAccountId"></param>
        /// <param name="orgNr"></param>
        /// <returns></returns>
        public async Task<List<PermissionCrudEnt>> LoadEffectivePermissionsInt(long userAccountId, long orgNr)
        {
            var perms = new Dictionary<int, PermissionCrudEnt>(); //permission number / crud
            try
            {
                var sql = "SELECT rp.permissionNr, rp.crud " +
                    "FROM base.rolePermission rp " +
                        "INNER JOIN base.userAccRole ur ON ur.roleId = rp.roleId " +
                        "INNER JOIN base.role r ON r.Id = rp.roleId " +
                    "WHERE ur.userAccId = @userAccId ";

                await Sql.Run(sql + "AND r.orgNr = @orgNr",
                    r =>
                    {
                        AddPermission(
                            GetInt(r, "permissionNr"),
                            GetString(r, "crud"),
                            perms);
                    },
                    new NpgsqlParameter("@userAccId", userAccountId),
                    new NpgsqlParameter("@orgNr", orgNr)
                );

                await Sql.Run(sql + "AND r.orgNr = " + GC.BaseOrgNr,
                    r => {
                        AddPermission(
                            GetInt(r, "permissionNr"),
                            GetString(r, "crud"),
                            perms);
                    },
                    new NpgsqlParameter("@userAccId", userAccountId)
                );
                
                return perms.Values.ToList<PermissionCrudEnt>();
            }
            catch
            {
                //ToDo Logme
                return new List<PermissionCrudEnt>();
            }
        }

        private void AddPermission (int nr, string crud, Dictionary<int, PermissionCrudEnt> perms)
        {
            if (!perms.ContainsKey(nr))
                perms[nr] = new PermissionCrudEnt
                {
                    Nr = nr,
                    Crud = crud
                };
            else
            {
                perms[nr].AddCrud(crud);
            }
        }


        /// <summary>
        /// Return a user's permission/crud settings
        /// The specifig org's permissions are used if exist, then org 0 permission's are the default
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orgNr"></param>
        /// <returns></returns>
        public async Task<List<RolePermissionCrudEnt>> GetPermissions(SessionEnt session)
        {
            var list = new List<RolePermissionCrudEnt>();
            try
            {
                var sql = "SELECT r.code as role, rp.permissionNr, rp.crud, r.orgNr " +
                    "FROM base.rolePermission rp " +
                        "INNER JOIN base.userAccRole uar ON uar.roleId = rp.roleId " +
                        "INNER JOIN base.role r ON r.Id = rp.roleId " +
                    "WHERE uar.userAccId = @userAccId ";
                var by = " ORDER BY r.code, rp.permissionNr ";

                await Sql.Run(sql + "AND r.orgNr = @orgNr" + by,
                    r => {
                        list.Add(new RolePermissionCrudEnt() { 
                            Role = GetStringNull(r, "role"),
                            PermissionNr = GetInt(r, "permissionNr"),
                            Crud = GetString(r, "crud"),
                            OrgNr = GetOrgNr(r)
                        });
                    },
                    new NpgsqlParameter("@userAccId", session.UserAccount.Id),
                    new NpgsqlParameter("@orgNr", session.Org.Nr)
                );

                await Sql.Run(sql + "AND r.orgNr = " + GC.BaseOrgNr + by,
                    r => {
                        list.Add(new RolePermissionCrudEnt()
                        {
                            Role = GetStringNull(r, "role"),
                            PermissionNr = GetInt(r, "permissionNr"),
                            Crud = GetString(r, "crud"),
                            OrgNr = GetOrgNr(r)
                        });
                    },
                    new NpgsqlParameter("@userAccId", session.UserAccount.Id)
                );
                return list.OrderBy(r => r.Role).ThenBy(r => r.PermissionNr).ToList();
            }
            catch
            {
                //ToDo Logme
                return new List<RolePermissionCrudEnt>();
            }
        }


        public bool IsAuthorizedToAccessEndPoint(SessionEnt session, PermissionAtt permAtt, CrudAtt crud)
        {
            if (permAtt == null) return true;
            if (session == null) return false;
            if (session.UserAccount.IsService()) return true;

            if (crud != null && crud.Action == GC.CrudIgnore) return true;

            var permEnt = GetPermissionEnt(permAtt.Nr);
            if (permEnt == null) return false;

            var userCrud = session.GetUserPermissionCrud(permEnt.Nr);
            if (userCrud == null) return false; //permission not found in user profile
            if (crud == null) return true;

            return userCrud.IndexOf(crud.Action) != -1;
        }
    }
}
