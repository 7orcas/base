
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using GC = Backend.GlobalConstants;

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
        private readonly LoginServiceI _loginService;
        private readonly AppDbContext _context;

        public UserRepo(AppDbContext context,
            LoginServiceI loginService,
            IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _loginService = loginService;
            _context = context;
        }

        public async Task<List<UserEnt>> GetList(UserSearch search)
        {
            var sql = "SELECT id, xxx AS Username, isActive, orgnrdefault AS OrgNrDefault "
                        + "FROM base.zzz "
                        + "WHERE id != " + GC.ServiceLoginId + " ";


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
            var user = null as UserEnt;

            if (userDto.IsNew())
                user = new UserEnt();
            else
                user = await GetById(userDto.Id);

            if (user == null)
                return null;
           
            //Cascade delete
            if (userDto.IsDelete)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return null;
            }

            //Update password
            if (!string.IsNullOrEmpty(userDto.PasswordNew))
            {
                user.Password = _loginService.PasswordHash(userDto.PasswordNew);
            }

            user.Encode();
            user.Username = userDto.Username;
            user.Email = userDto.Email;
            user.IsEmailVerified = userDto.IsEmailVerified;
            user.LangCode = userDto.LangCode;
            user.OrgNrDefault = userDto.OrgNr;
            user.Attempts = userDto.Attempts;
            user.IsAdminUser = userDto.IsAdminUser;

            user.IsActive = userDto.IsActive;
            VersionIncrement(user);

            foreach (var accountDto in userDto.Accounts ?? new List<UserDto.UserAccountDto>())
            {
                var account = null as UserAccountEnt;

                // Add new account
                if (accountDto.IsNew())
                {
                    account = new UserAccountEnt()
                    {
                        User = user,
                        OrgNr = accountDto.OrgNr,
                    };
                    user.Accounts.Add(account);
                }
                else
                    account = user.Accounts.FirstOrDefault(a => a.Id == accountDto.Id);


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
                            //UserAccountId = account.Id,
                            RoleId = accRoleDto.RoleId,
                        };
                        account.Roles.Add(accRole);
                    }

                    // Update account role
                    if (accRole != null)
                    {
                        accRole.FromDate = accRoleDto.FromDate;
                        accRole.ToDate = accRoleDto.ToDate;
                        accRole.IsActive = accRoleDto.IsActive;
                        VersionIncrement(accRole);
                    }
                }
            }
            if (userDto.IsNew())
                _context.Users.Add(user);
            
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
