
using Backend.Data;
using DocumentFormat.OpenXml.Math;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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

        public async Task<List<UserEnt>> GetList(UserSearch search, int orgNr)
        {
            var includeRole = !string.IsNullOrWhiteSpace(search.Role);

            var sql = "SELECT DISTINCT z.id, xxx AS Username, z.isActive, z.attempts, z.orgnrdefault AS OrgNrDefault "
                        + "FROM base.zzz z ";

            if (includeRole)
            {
                sql += @"JOIN base.userAcc ua ON ua.zzzid = z.id
                         JOIN base.userAccRole uar ON uar.userAccId = ua.id AND uar.isActive is TRUE ";
                
                if (search.IncludeEffectDate)
                    sql += @"AND (uar.fromDate IS NULL OR uar.fromDate <= CURRENT_DATE)
                             AND (uar.toDate IS NULL OR uar.toDate >= CURRENT_DATE) ";

                sql += " JOIN base.role r ON r.id = uar.roleId ";
            }

            sql += "WHERE z.id != " + GC.ServiceLoginId + " ";

            var parameters = new DynamicParameters();

            sql += " AND z.orgnrdefault = @OrgNr";
            parameters.Add("OrgNr", orgNr);

            if (includeRole)
                sql += GetSqlAndClause(search.Role, "Role", parameters, "r.Code", search);
            
            sql += GetSqlAndClause(search.Username, "Username", parameters, "z.xxx", search);
            sql += GetSqlAndClause(search.Email, "Email", parameters, "z.email", search);

            sql += GetSqlWhereClauseForSearchActive(search, "z")
                + " ORDER BY z.xxx"
                + GetSqlLimitClauseForSearch(search);


            return await GetList<UserEnt>(sql, parameters);
        }

        public async Task<VersionInfo?> GetVersion(long id)
        {
            return await GetVersion(id, "base.zzz");
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

        public async Task<UserEnt> Update(UserEnt user, UserDto dto)
        {
            //Cascade delete
            if (dto.IsDelete)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return null;
            }

            //Update password
            if (!string.IsNullOrEmpty(dto.PasswordNew))
            {
                user.Password = _loginService.PasswordHash(dto.PasswordNew);
            }
                        
            user.Username = dto.Username;
            user.Email = dto.Email;
            user.IsEmailVerified = dto.IsEmailVerified;
            user.LangCode = dto.LangCode;
            user.OrgNrDefault = dto.OrgNr;
            user.Attempts = dto.Attempts;
            user.IsAdminUser = dto.IsAdminUser;

            user.IsActive = dto.IsActive;
            VersionIncrement(user);

            foreach (var accountDto in dto.Accounts ?? new List<UserDto.UserAccountDto>())
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

            user.Encode();

            if (dto.IsNew())
                _context.Users.Add(user);
            
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
