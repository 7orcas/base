using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Caching.Memory;
using Npgsql;
using Org.BouncyCastle.Asn1.Ocsp;
using Superpower.Model;
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
        private readonly UserRepoI _userRepo;

        public UserService(IServiceProvider serviceProvider,
            LabelServiceI labelService,
            OrgServiceI orgService,
            UserRepoI userRepo) 
            : base(serviceProvider) 
        {
            _labelService = labelService;
            _orgService = orgService;
            _userRepo = userRepo;
        }

        public async Task<List<UserEnt>> GetUserList()
        {
            return await _userRepo.GetList();
        }

        public async Task<UserEnt?> GetUser(long id)
        {
            return await _userRepo.GetById(id);
        }

        public async Task<UserEnt?> UpdateUser(UserDto user)
        {
            return await _userRepo.Update(user);
        }

        public async Task<UserDto> Populate(UserEnt user)
        {

            UserDto userDto = new UserDto()
            {
                Id = user.Id,
                Username = user.Username,
                IsEmailVerified = user.IsEmailVerified,
                OrgNrDefault = user.OrgNrDefault,
                LangCode = user.LangCode,
                Attempts = user.Attempts,
                AttemptsLockout = user.AttemptsLockout,
                LastLogin = user.LastLogin,
                IsActive = user.IsActive,
                IsMfaRequired = user.IsMfaRequired,
                IsMfaEnabled = user.IsMfaEnabled,
                MfaSecret = user.MfaSecret,
                Updated = user.Updated,
                Version = user.Version,
                Accounts = new List<UserDto.UserAccountDto>()
            };

            if (user.Accounts == null) return userDto;

            foreach (var a in user.Accounts)
                userDto.Accounts.Add(await Populate(a));

            return userDto;
        }

        public async Task<UserDto.UserAccountDto> Populate(UserAccountEnt account)
        {
            var org = await _orgService.GetOrg(account.OrgNr);

            UserDto.UserAccountDto userAccountDto = new UserDto.UserAccountDto()
            {
                Id = account.Id,
                UserId = account.UserId,
                orgNr = account.OrgNr,
                OrgCode = org.Code,
                IsActive = account.IsActive,
                IsAdminUser = account.IsAdminUser,
                IsAdminLang = account.IsAdminLang,
                Classification = account.Classification,
                LastLogin = account.LastLogin,
                Updated = account.Updated,
                Version = account.Version
            };

            return userAccountDto;
        }




    }
}
