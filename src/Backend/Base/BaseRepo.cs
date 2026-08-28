namespace Backend.Base
{
    public abstract class BaseRepo : SqlUtils
    {
        protected readonly Serilog.ILogger _log;

        public BaseRepo(IServiceProvider serviceProvider)
        {
            _log = Log.Logger;
        }

        protected void VersionIncrement(VersionI entity)
        {
            if (entity.Version == null)
                entity.Version = 0;
            
            entity.Version++;
            entity.Updated = DateTimeOffset.UtcNow;
        }

        public async Task<List<T>> GetList<T>(string sql) 
        {
            try
            {
                using var conn = Sql.GetConnection();
                var result = await conn.QueryAsync<T>(sql);
                return result.ToList();
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
