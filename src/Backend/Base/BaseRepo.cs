using GC = Backend.GlobalConstants;

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

        public async Task<VersionInfo> GetVersion(int nr, string table)
        {
            return await GetVersion(Convert.ToInt64(nr), table, "nr");
        }

        public async Task<VersionInfo> GetVersion(long id, string table)
        {
            return await GetVersion(id, table, "id");
        }

        public async Task<VersionInfo> GetVersion(long id, string table, string field)
        {
            var sql = "SELECT Version, Updated "
                    + "FROM " + table + " "
                    + "WHERE " + field + " = @Id";
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

        public async Task<string> GetCode(long id, string table)
        {
            var sql = "SELECT Code "
                     + "FROM " + table + " "
                     + "WHERE id = @Id";
            try
            {
                using var conn = Sql.GetConnection();
                var result = await conn.QuerySingleAsync<string>(sql, new { Id = id });
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

        [Obsolete("Use GetList<T>(string sql, object? parameters = null) instead.", false)]
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

        public async Task<List<T>> GetList<T>(string sql, object? parameters = null)
        {
            try
            {
                using var conn = Sql.GetConnection();
                var result = await conn.QueryAsync<T>(sql, parameters);
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

        public string SqlParameter(string value, _BaseSearch search)
        {
            switch (search.TextFieldSearchType)
            {
                case GC.TextSearchExtact:
                    return value;
                case GC.TextSearchStart:
                    return $"{value}%";
                case GC.TextSearchContains:
                    return $"%{value}%";
                default:
                    return value;
            }
        }

        public string SqlUnaccent(string column, string parameter, _BaseSearch search)
        {
            if (search.IsTextFieldSearchUnaccent)
                return " immutable_unaccent(lower(" + column + ")) LIKE immutable_unaccent(lower(@" + parameter + ")) ";
            
            return " " + column + " LIKE @" + parameter + " ";
        }

    }

    public class VersionInfo : VersionI
    {
        public string? Code { get; set; }
        public int Version { get; set; }
        public DateTimeOffset Updated { get; set; }
    }

}
