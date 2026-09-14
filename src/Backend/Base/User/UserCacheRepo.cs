using System.Text.Json;


namespace Backend.Base.User
{
    public class UserCacheRepo : BaseRepo, UserCacheRepoI
    {

        public UserCacheRepo(IServiceProvider serviceProvider)
            : base(serviceProvider) {}


        public async Task SaveCache(long userAccId, string cacheKey, string json)
        {
            await RunSql("DELETE FROM base.userAccCache "
                        + "WHERE userAccId = " + userAccId + " "
                        + "AND cacheKey = '" + cacheKey + "'");

            await RunSql("INSERT INTO base.userAccCache " +
                "(userAccId, cacheKey, details) " +
                "VALUES (" + userAccId + ", '" + cacheKey + "', '" + json + "')");
        }

        public async Task<T?> GetCache<T>(long userAccId, string cacheKey) 
        {
            try
            {
                const string sql = @"SELECT details
                                FROM base.useracccache
                                WHERE useraccid = @UserAccId
                                  AND cachekey = @CacheKey";

                using var conn = Sql.GetConnection();

                var json = await conn.QuerySingleOrDefaultAsync<string>(
                    sql,
                    new
                    {
                        UserAccId = userAccId,
                        CacheKey = cacheKey
                    });

                if (string.IsNullOrWhiteSpace(json))
                    return default(T);

                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        private async Task RunSql(string sql) 
        {
            try
            {
                using var conn = Sql.GetConnection();
                var result = await conn.ExecuteAsync(sql);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(
                    ex,
                    "Sql failed: {Sql}",
                    sql);
                throw;
            }
        }

    }
}
