using Common.Validator;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.Data;
using System.Text;
using System.Transactions;
using GC = Backend.GlobalConstants;

namespace Backend.Base.Role
{
    /// <summary>
    /// Manage loading and processing roles
    /// </summary>
    /// <author>John Stewart</author>
    /// <created>April 22, 2025</created>
    /// <license>**Licence**</license>
    public class RoleService: BaseService, RoleServiceI
    {
        private readonly RoleRepoI _roleRepo;


        public RoleService(IServiceProvider serviceProvider, RoleRepoI roleRepo) 
            : base(serviceProvider) 
        { 
            _roleRepo = roleRepo;
        }

        /// <summary>
        /// Return a user's Roles
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task<List<UserAccountRoleEnt>> GetUserRoles(SessionEnt session)
        {
            var list = new List<UserAccountRoleEnt>();
            try
            {
                var sql = "SELECT r.id, r.code, r.descr, r.orgNr, zr.updated, zr.isActive " +
                    "FROM base.userAccRole zr " +
                        "INNER JOIN base.role r ON r.Id = zr.roleId " +
                    "WHERE zr.userAccId = @userId " +
                    "AND r.isActive = true ";
                var by = " ORDER BY r.code ";

                await Sql.Run(sql + "AND r.orgNr = @orgNr" + by,
                    r => {
                        list.Add(new UserAccountRoleEnt() {
                            RoleId = GetId(r),
                            Code = GetCode(r),
                            Description = GetDescription(r),
                            orgNr = GetOrgNr(r),
                            Updated = GetUpdated(r),
                            IsActive = IsActive(r),
                        });
                    },
                    new NpgsqlParameter("@userId", session.UserAccount.Id),
                    new NpgsqlParameter("@orgNr", session.Org.Nr)
                );

                await Sql.Run(sql + "AND r.orgNr = " + GC.BaseOrgNr + by,
                    r => {
                        list.Add(new UserAccountRoleEnt()
                        {
                            RoleId = GetId(r),
                            Code = GetCode(r),
                            Description = GetDescription(r),
                            orgNr = GetOrgNr(r),
                            Updated = GetUpdated(r),
                            IsActive = IsActive(r),
                        });
                    },
                    new NpgsqlParameter("@userId", session.UserAccount.Id)
                );

                return list.OrderBy(r => r.Code).ToList();
            }
            catch
            {
                //ToDo Logme
                return new List<UserAccountRoleEnt>();
            }
        }

        /// <summary>
        /// Return all Roles for the org and base org (nr=0)
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task<List<RoleEnt>> GetRoles(SessionEnt session)
        {
            var list = new List<RoleEnt>();
            try
            {
                var sql = "SELECT * FROM base.role r ";
                var by = " ORDER BY r.code ";

                await Sql.Run(sql + "WHERE r.orgNr = @orgNr" + by,
                    r =>  list.Add(ReadBaseEntity<RoleEnt>(r)),
                    new NpgsqlParameter("@orgNr", session.Org.Nr)
                );

                await Sql.Run(sql + "WHERE r.orgNr = " + GC.BaseOrgNr + by,
                    r => list.Add(ReadBaseEntity<RoleEnt>(r))
                );

                return list.OrderBy(r => r.Code).ToList();
            }
            catch
            {
                //ToDo Logme
                return new List<RoleEnt>();
            }
        }

        /// <summary>
        /// Return the specific role for the passed in id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<RoleEnt> GetRole(long roleId)
        {
            try
            {
                var sql = "SELECT r.* FROM base.role r WHERE r.id = @id ";

                RoleEnt ent = null;
                await Sql.Run(sql,
                    r => ent = ReadBaseEntity<RoleEnt>(r),
                    new NpgsqlParameter("@id", roleId)
                );
                ent.RolePermissions = new List<RolePermissionEnt>();

                sql = "SELECT rp.* FROM base.rolePermission rp WHERE rp.roleId = @id ";
                await Sql.Run(sql,
                    r => {
                        ent.RolePermissions.Add(new RolePermissionEnt
                        {
                            Id = GetId(r),
                            RoleId = GetId(r),
                            PermissionNr = GetInt(r, "permissionNr"),
                            Crud = GetString(r, "crud"),
                            Updated = GetUpdated(r),
                            IsActive = IsActive(r),
                        });
                    },
                    new NpgsqlParameter("@id", roleId)
                );

                return ent;
            }
            catch
            {
                //ToDo Logme
                return null;
            }
        }


        /// <summary>
        /// Update and/or Insert the list of roles
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task SaveRoles(List<RoleEnt> list, List<RoleEnt> listDel, SessionEnt session)
        {
            foreach (var ent in listDel.Where(e => !e.IsNew()))
            {
                await DeleteRolePermissions(ent);
                await DeleteRole(ent);
            }
            foreach (var ent in list.Where(e => !e.IsNew()))
            {
                await DeleteRolePermissions(ent);
                await UpdateRole(ent);
                await InsertRolePermissions(ent);
            }
            foreach (var ent in list.Where(e => e.IsNew()))
            {
                //await InsertRole(ent);
                ent.Id = 0;
                ent.Updated = DateTime.UtcNow;
                //ent.RolePermissions = new List<RolePermissionEnt>(); //not needed
                var entX = await _roleRepo.Create(ent);
                await InsertRolePermissions(entX);
            }
        }

        private async Task UpdateRole(RoleEnt ent)
        {
            ent.Encode();
            await Sql.ExecuteAsync(
                    "UPDATE base.role " +
                    "SET " + Update(ent) +
                    " WHERE id = " + ent.Id
            );
        }

        private async Task InsertRole(RoleEnt ent)
        {
            ent.Encode();
            var id = await Sql.ExecuteAndReturnId("INSERT INTO base.role " + Insert(ent));
            ent.Id = id;
        }

        private async Task InsertRolePermissions(RoleEnt ent)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var p in ent.RolePermissions)
            {
                if (!string.IsNullOrEmpty(p.Crud))
                    sb.Append(
                        "INSERT INTO base.rolePermission (roleId,PermissionNr,crud,updated) " +
                        "VALUES (" + 
                        ent.Id + "," +
                        p.PermissionNr + "," +
                        (p.Crud != null ? "'" + p.Crud + "'," : "") +
                        "'" + DateTime.Now.ToString(GC.DateTimeFormat) + "'" +
                        ");");
            }
            var sql = sb.ToString();
            if (!string.IsNullOrEmpty(sql))
                await Sql.ExecuteAsync(sql);
        }

        private async Task DeleteRole(RoleEnt ent)
        {
            await Sql.ExecuteAsync("DELETE FROM base.userAccRole WHERE roleId = " + ent.Id);
            await Sql.ExecuteAsync("DELETE FROM base.role WHERE Id = " + ent.Id);
        }

        private async Task DeleteRolePermissions(RoleEnt ent)
        {
            await Sql.ExecuteAsync("DELETE FROM base.rolePermission WHERE roleId = " + ent.Id);
        }
    }
}
