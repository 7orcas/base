using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Backend.Base.User
{
    public class UserCacheService : BaseService, UserCacheServiceI
    {
        private readonly UserCacheRepoI _userCacheRepo;

        public UserCacheService(IServiceProvider serviceProvider,
            UserCacheRepoI userCacheRepo)
            : base(serviceProvider)
        {
            _userCacheRepo = userCacheRepo;
        }

        public async Task SaveCache(SessionEnt session, string cacheKey, string json)
        {
            await _userCacheRepo.SaveCache(session.UserAccount.Id, cacheKey, json);
        }

        public async Task<T?> GetCache<T>(SessionEnt session, string cacheKey) 
        {
            return await _userCacheRepo.GetCache<T>(session.UserAccount.Id, cacheKey);
        }


    }
}
