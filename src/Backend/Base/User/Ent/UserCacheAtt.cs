namespace Backend.Base.Permission.Ent
{
    [AttributeUsage (AttributeTargets.Method)]
    public class UserCacheAtt : Attribute
    {
        public string CacheKey { get; }

        public UserCacheAtt(string cacheKey)
        {
            CacheKey = cacheKey;
        }
    }
}
