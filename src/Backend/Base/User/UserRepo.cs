
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;

namespace Backend.Base.User
{
    /// <summary>
    /// Manage User database activities
    /// </summary>
    /// <author>John Stewart</author>
    /// <created>May 11, 2026</created>
    /// <license>**Licence**</license>
    public class UserRepo : BaseRepo, UserRepoI
    {
        private readonly AppDbContext _context;

        public UserRepo(AppDbContext context,
            IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _context = context;
        }

        public async Task<List<UserEnt>> GetList(UserSearch search)
        {
            var sql = "SELECT id, xxx AS Username, isActive, orgnrdefault AS OrgNrDefault "
                        + "FROM base.zzz "
                        + "WHERE 1=1";


    if (!string.IsNullOrEmpty(search.Username))
    {
        sql += " AND xxx LIKE '%" + search.Username + "%'";
    }

     sql += " ORDER BY xxx";

            return await GetList<UserEnt>(sql);
        }


        public async Task<UserEnt> Create(UserEnt User)
        {
            _context.Users.Add(User);
            await _context.SaveChangesAsync();
            return User;
        }

        public async Task<UserEnt?> GetById(long id)
        {
            try
            {
                return await _context.Users
                    .Include(u => u.Accounts)
                    .FirstOrDefaultAsync(x => x.Id == id);

            }
            catch (Exception ex)
            {
                _log.Error($"Error retrieving user with ID {id}", ex);
                return null;
            }
        }

        public async Task<UserEnt?> Update(UserDto user)
        {
            var existingUser = await GetById(user.Id);

            if (existingUser == null)
            {
                return null;
            }
            existingUser.Encode();
            existingUser.Username = user.Username;
            existingUser.OrgNrDefault = user.OrgNr;
            existingUser.Attempts = user.Attempts;

            existingUser.IsActive = user.IsActive;
            VersionIncrement(existingUser);

            foreach (var accountDto in user.Accounts ?? new List<UserDto.UserAccountDto>())
            {
                var existingAccount = existingUser.Accounts.FirstOrDefault(a => a.Id == accountDto.Id);
                if (existingAccount != null)
                {
                    // Update existing account
                    existingAccount.LastLogin = accountDto.LastLogin;
                    existingAccount.Classification = accountDto.Classification;
                    existingAccount.IsAdminUser = accountDto.IsAdminUser;
                    existingAccount.IsAdminLang = accountDto.IsAdminLang;
                    VersionIncrement(existingAccount);
                    // Update roles
                    //foreach (var roleDto in accountDto.Roles ?? new List<UserDto.UserAccountRoleDto>())
                    //{
                    //    var existingRole = existingAccount.Roles.FirstOrDefault(r => r.Id == roleDto.Id);
                    //    if (existingRole != null)
                    //    {
                    //        existingRole.RoleId = roleDto.RoleId;
                    //        existingRole.Role = roleDto.Role;
                    //    }
                    //    else
                    //    {
                    //        // Add new role
                    //        existingAccount.Roles.Add(new UserAccountRoleEnt
                    //        {
                    //            RoleId = roleDto.RoleId,
                    //            Role = roleDto.Role
                    //        });
                    //    }
                    //}
                }
                else
                {
                    // Add new account
                    var newAccount = new UserAccountEnt
                    {
                        UserId = user.Id,
                        OrgNr = accountDto.OrgNr,
                        LastLogin = accountDto.LastLogin,
                        Classification = accountDto.Classification,
                        IsAdminUser = accountDto.IsAdminUser,
                        IsAdminLang = accountDto.IsAdminLang,

                        //Roles = accountDto.Roles?.Select(r => new UserAccountRoleEnt
                        //{
                        //    RoleId = r.RoleId,
                        //    Role = r.Role
                        //}).ToList() ?? new List<UserAccountRoleEnt>()
                    };
                    VersionIncrement(newAccount);
                    existingUser.Accounts.Add(newAccount);
                }
            }

            //Update all fields
            //_context.Entry(existingUser).CurrentValues.SetValues(user);
            // Update any other properties you want to allow changes to

            await _context.SaveChangesAsync();

            return existingUser;
        }
    }
}
