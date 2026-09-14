namespace Backend.Base.User
{
    public interface UserCacheServiceI
    {
        Task SaveCache(SessionEnt session, string cacheKey, string json);
        Task<T?> GetCache<T>(SessionEnt session, string cacheKey);
    }
}
