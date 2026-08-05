using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Caching.Memory;
using Npgsql;
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
        private readonly UserRepoI _userRepo;

        public UserService(IServiceProvider serviceProvider,
            LabelServiceI labelService,
            UserRepoI userRepo) 
            : base(serviceProvider) 
        {
            _labelService = labelService;
            _userRepo = userRepo;
        }

        public async Task<List<UserEnt>> GetUserList()
        {
            var list = new List<UserEnt>();
            await Sql.Run(
                    "SELECT * FROM base.zzz ",
                    r => {
                        var user = new UserEnt();
                        user.Id = GetId(r);
                        user.Username = GetString(r, "xxx");
                        list.Add(user);
                    }
                );
            return list;
        }

        public async Task<UserEnt?> GetUser(long id)
        {
            return await _userRepo.GetById(id);
        }

        public async Task<UserEnt?> UpdateUser(UserDto user)
        {
            return await _userRepo.Update(user);
        }

        public UserDto Populate(UserEnt user)
        {
            UserDto userDto = new UserDto()
            {
                UserId = user.Id,
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
                Version = user.Version
            };
            return userDto;
        }

    }
}
