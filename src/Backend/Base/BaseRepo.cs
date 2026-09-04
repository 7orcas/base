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

        public async Task<VersionInfo> GetVersion(long id, string table)
        {
            var sql = "SELECT Version, Updated "
                    + "FROM " + table + " "
                    + "WHERE id = @Id";
            try
            {
                using var conn = Sql.GetConnection();
                var result = await conn.QuerySingleAsync<VersionInfo>(sql, new { Id = id });
                return result;
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

        public async Task<VersionInfo> GetVersion(int nr, string table)
        {
            var sql = "SELECT Version, Updated "
                    + "FROM " + table + " "
                    + "WHERE nr = @Nr";
            try
            {
                using var conn = Sql.GetConnection();
                var result = await conn.QuerySingleAsync<VersionInfo>(sql, new { Nr = nr });
                return result;
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

        public string GetSqlWhereClauseForSearchActive(_BaseSearch search)
        {
            var sql = "";
            if (search.IncludeActive && !search.IncludeInActive)
            {
                sql += " AND isActive = TRUE ";
            }
            if (!search.IncludeActive && search.IncludeInActive)
            {
                sql += " AND isActive = FALSE ";
            }
            return sql;
        }

        public string GetSqlLimitClauseForSearch(_BaseSearch search)
        {
            var sql = "";
            if (search.MaxRecordsReturned > 0)
            {
                sql += " LIMIT " + search.MaxRecordsReturned;
            }
            return sql;
        }

    }

    public class VersionInfo : VersionI
    {
        public int Version { get; set; }
        public DateTimeOffset Updated { get; set; }
    }

}
