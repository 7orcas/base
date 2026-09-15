namespace Backend.Base.User
{
    public interface UserCacheRepoI
    {
        Task SaveCache(long userAccId, string cacheKey, string json);
        Task DeleteCache(long userAccId, string cacheKey);
        Task<T?> GetCache<T>(long userAccId, string cacheKey);
    }
}
