using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Caching.Memory;
using Npgsql;
using Org.BouncyCastle.Asn1.Ocsp;
using Superpower.Model;
using System.Runtime.ConstrainedExecution;
using GC = Backend.GlobalConstants;

/// <summary>
/// User methods
/// Created: August 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>


namespace Backend.Base.User
{
    public class UserService: BaseService, UserServiceI
    {
        private readonly LabelServiceI _labelService;
        private readonly OrgServiceI _orgService;
        private readonly RoleServiceI _roleService;
        private readonly PermissionServiceI _permissionService;
        private readonly PermissionInitialiseServiceI _permissionInitialiseService;
        private readonly UserRepoI _userRepo;

        public UserService(IServiceProvider serviceProvider,
            LabelServiceI labelService,
            OrgServiceI orgService,
            RoleServiceI roleService,
            PermissionServiceI permissionService,
            PermissionInitialiseServiceI permissionInitialiseService,
            UserRepoI userRepo) 
            : base(serviceProvider) 
        {
            _labelService = labelService;
            _orgService = orgService;
            _roleService = roleService;
            _permissionService = permissionService;
            _permissionInitialiseService = permissionInitialiseService;
            _userRepo = userRepo;
        }

        public async Task<List<UserEnt>> GetUserList(UserSearch search)
        {
            return await _userRepo.GetList(search);
        }

        public async Task<UserEnt?> GetUser(long id)
        {
            return await _userRepo.GetById(id);
        }

        public async Task<UserEnt?> UpdateUser(UserDto user)
        {
            return await _userRepo.Update(user);
        }

        public async Task<UserDto?> NewUser(SessionEnt session)
        {
            var user = new UserEnt()
            {
                Id = GetTempId(),
                OrgNrDefault = session.Org.Nr,
                Username = "",
                LangCode = session.Org.LangCode,
                //IsMfaRequired = org.IsMfaRequired, ToDo
                //IsMfaEnabled = user.IsMfaEnabled, ToDo
                IsActive = true,
                Version = GC.NewRecordVersion,
                Accounts = new List<UserAccountEnt>()
            };
            user.Accounts.Add(NewAccount(user, session));

            return await Populate(user);
        }

        private UserAccountEnt NewAccount(UserEnt user, SessionEnt session)
        {
            return new UserAccountEnt()
            {
                Id = GetTempId(),
                UserId = user.Id,
                OrgNr = session.Org.Nr,
                IsActive = true,
                IsAdminLang = false,
                Classification = 0,
                Version = GC.NewRecordVersion
            };
        }


        public async Task<UserDto> PopulateList(UserEnt user)
        {
            UserDto userDto = new UserDto()
            {
                Id = user.Id,
                Username = user.Username,
                IsActive = user.IsActive,
                Updated = user.Updated,
                Version = user.Version,
            };

            return userDto;
        }

        public async Task<UserDto> Populate(UserEnt user)
        {
            var org = await _orgService.GetOrg(user.OrgNrDefault);
            var labels = await _labelService.GetLangCodeDic(user.LangCode, org.LangLabelVariant);

            UserDto userDto = new UserDto()
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                IsEmailVerified = user.IsEmailVerified,
                OrgNr = user.OrgNrDefault,
                LangCode = user.LangCode,
                Attempts = user.Attempts,
                AttemptsLockout = user.AttemptsLockout,
                LastLogin = user.LastLogin,
                IsActive = user.IsActive,
                IsAdminUser = user.IsAdminUser,
                IsMfaRequired = user.IsMfaRequired,
                IsMfaEnabled = user.IsMfaEnabled,
                MfaSecret = user.MfaSecret,
                Updated = user.Updated,
                Version = user.Version,
                Accounts = new List<UserDto.UserAccountDto>()
            };

            if (user.Accounts == null) return userDto;

            foreach (var a in user.Accounts)
                userDto.Accounts.Add(await Populate(a, labels));

            return userDto;
        }

        private async Task<UserDto.UserAccountDto> Populate(UserAccountEnt account, Dictionary<string, string> labels)
        {
            var org = await _orgService.GetOrg(account.OrgNr);
            var roles = await _roleService.GetRoles(org.Nr);
            var rolesById = roles.ToDictionary(r => r.Id);
            var perms = await _permissionService.LoadEffectivePermissionsInt(account.Id, org.Nr);
            var permDic = _permissionInitialiseService.GetPermissions();

            UserDto.UserAccountDto accountDto = new UserDto.UserAccountDto()
            {
                Id = account.Id,
                UserId = account.UserId,
                OrgNr = account.OrgNr,
                OrgCode = org.Code,
                IsActive = account.IsActive,
                IsAdminLang = account.IsAdminLang,
                Classification = account.Classification,
                LastLogin = account.LastLogin,
                Updated = account.Updated,
                Version = account.Version,
                Roles = new List<UserDto.UserAccountRoleDto>(),
                Permissions = new List<UserDto.UserAccountPermissionDto>()
            };

            foreach (var a in account.Roles)
                accountDto.Roles.Add(await Populate(a));

            //update role info and add in all other roles for the org that the user does not have assigned
            foreach (var role in roles)
            {
                var r = accountDto.Roles.Find(r => r.RoleId == role.Id);

                if (r != null)
                {
                    r.Code = role.Code;
                    r.Description = role.Description;
                    r.IsRoleActive = role.IsActive;
                }
                else
                {
                    accountDto.Roles.Add(new UserDto.UserAccountRoleDto
                    {
                        RoleId = role.Id,
                        Code = role.Code,
                        Description = role.Description,
                        IsRoleActive = role.IsActive
                    });
                }
            }

            //Remove any roles that are not in the org or the base org
            accountDto.Roles = accountDto.Roles.Where(r => r.Code != null).ToList();
            accountDto.Roles.Sort((x, y) => x.Code.CompareTo(y.Code));

            foreach (var perm in perms)
            {
                var lk = "?";
                if (permDic.ContainsKey(perm.Nr))
                {
                    lk = (permDic[perm.Nr]).LangKey;
                    lk = GetLabel(lk, labels);
                }

                accountDto.Permissions.Add(new UserDto.UserAccountPermissionDto
                {
                    PermissionNr = perm.Nr,
                    Code = lk,
                    Crud = perm.Crud
                });
            }

            return accountDto;
        }

        private async Task<UserDto.UserAccountRoleDto> Populate(UserAccountRoleEnt role)
        {
            UserDto.UserAccountRoleDto roleDto = new UserDto.UserAccountRoleDto()
            {
                Id = role.Id,
                RoleId = role.RoleId,
                FromDate = role.FromDate,
                ToDate = role.ToDate,
                IsActive = role.IsActive,
                Updated = role.Updated,
                Version = role.Version
            };

            return roleDto;
        }



    }
}
