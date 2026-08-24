
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
                    .ThenInclude(a => a.Roles)
                    .FirstOrDefaultAsync(x => x.Id == id);

            }
            catch (Exception ex)
            {
                _log.Error($"Error retrieving user with ID {id}", ex);
                return null;
            }
        }

        public async Task<UserEnt?> Update(UserDto userDto)
        {
            var user = await GetById(userDto.Id);

            if (user == null)
            {
                return null;
            }
            user.Encode();
            user.Username = userDto.Username;
            user.Email = userDto.Email;
            user.OrgNrDefault = userDto.OrgNr;
            user.Attempts = userDto.Attempts;
            user.IsAdminUser = userDto.IsAdminUser;

            user.IsActive = userDto.IsActive;
            VersionIncrement(user);

            foreach (var accountDto in userDto.Accounts ?? new List<UserDto.UserAccountDto>())
            {
                var account = user.Accounts.FirstOrDefault(a => a.Id == accountDto.Id);

                // Add new account
                if (account == null)
                {
                    account = new UserAccountEnt
                    {
                        User = user,
                        UserId = userDto.Id,
                        OrgNr = accountDto.OrgNr,
                    };
                    user.Accounts.Add(account);
                }

                // Update account
                account.Encode();
                account.Classification = accountDto.Classification;
                account.IsAdminLang = accountDto.IsAdminLang;
                account.IsActive = accountDto.IsActive;
                VersionIncrement(account);

                // Update roles
                foreach (var accRoleDto in accountDto.Roles ?? new List<UserDto.UserAccountRoleDto>())
                {
                    var accRole = account.Roles.FirstOrDefault(r => r.RoleId == accRoleDto.RoleId);
                    
                    // Add new account role if assigned
                    if (accRole == null && accRoleDto.IsActive)
                    {
                        accRole = new UserAccountRoleEnt
                        {
                            UserAccount = account,
                            UserAccountId = account.Id,
                            RoleId = accRoleDto.RoleId,
                        };
                        account.Roles.Add(accRole);
                    }

                    // Update account role
                    if (accRole != null)
                    {
                        accRole.IsActive = accRoleDto.IsActive;
                        VersionIncrement(accRole);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return user;
        }
    }
}
