using Backend.Data;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Npgsql;
using GC = Backend.GlobalConstants;

namespace Backend.Base.LabelAdmin
{
    /// <summary>
    /// Manage Language
    /// </summary>
    /// <author>John Stewart</author>
    /// <created>November, 2026</created>
    /// <license>**Licence**</license>
    public class LabelAdminRepo : BaseRepo, LabelAdminRepoI
    {
        private readonly AppDbContext _context;
      

        public LabelAdminRepo(AppDbContext context,
            IServiceProvider serviceProvider,
            OrgConfigInitialiseServiceI orgConfigInitialiseService)
            : base(serviceProvider)
        {
            _context = context;
        }


        public async Task<List<LangCodeAdmin>> GetLangCodeList(LabelSearch search)
        {
            var parameters = new DynamicParameters();
            var includeLangCode = !string.IsNullOrWhiteSpace(search.LangCode);

            var sql = "SELECT DISTINCT c.id, c.code, c.descr AS Description, c.flag, c.updated, c.isactive " +
                       "FROM base.langCode AS c " +
                       "WHERE c.id > 0 ";

            if (includeLangCode)
                sql += GetSqlAndClause(search.LangCode, "LangCode", parameters, "c.code", search);

            sql += " ORDER BY code";
            return await GetList<LangCodeAdmin>(sql, parameters);
        }

        public async Task<List<LangKeyAdmin>> GetLangKeyList(LabelSearch search)
        {
            var parameters = new DynamicParameters();
            var includeLangKey = !string.IsNullOrWhiteSpace(search.LangKey);
            var includeVariant = search.Variant.HasValue;

            var sql = "SELECT DISTINCT k.id, k.code, k.descr AS Description, k.updated, k.version, l.variant " +
                       "FROM base.langKey AS k " +
                       "JOIN base.langLabel AS l ON l.langKeyId = k.Id " +
                       "WHERE k.id > 0 ";
            
            if (includeLangKey)
                sql += GetSqlAndClause(search.LangKey, "LangKey", parameters, "k.code", search);

            if (includeVariant)
                sql += GetSqlAndClause(search.Variant.Value, "Variant", parameters, "l.variant");

            sql += " ORDER BY code";
            return await GetList<LangKeyAdmin>(sql, parameters);
        }


        public async Task<List<LangLabelAdmin>> GetLangLabelList(LabelSearch search)
        {
            var parameters = new DynamicParameters();
            var includeLangKey = !string.IsNullOrWhiteSpace(search.LangKey);
            var includeLangLabel = !string.IsNullOrWhiteSpace(search.LangLabel);
            var includeVariant = search.Variant.HasValue;

            var sql = "SELECT DISTINCT " +
                        "c.id as LangCodeId, " +
                        "l.langKeyId, " +
                        "l.id as Id, " +
                        "l.langCode, " +
                        "k.code AS LangKey, " +
                        "l.variant, " +
                        "l.code AS Label " +
                       "FROM base.langLabel AS l " +
                           "JOIN base.langKey AS k ON k.Id = l.langKeyId " +
                           "JOIN base.langCode AS c ON c.code = l.langCode " +
                       "WHERE c.id > 0 ";

            sql += GetSqlAndClause(search.LangCodeId, "LangCodeId", parameters, "c.id");
            
            if (includeLangKey)
                sql += GetSqlAndClause(search.LangKey, "LangKey", parameters, "k.code", search);

            if (includeLangLabel)
                sql += GetSqlAndClause(search.LangLabel, "Label", parameters, "l.code", search);

            if (includeVariant)
                sql += GetSqlAndClause(search.Variant.Value, "Variant", parameters, "l.variant");

            sql += " ORDER BY k.code, l.variant ";
            return await GetList<LangLabelAdmin>(sql, parameters);
        }

        public async Task<long> IsExist(LangLabelNewDto dto)
        {
            var parameters = new DynamicParameters();

            var sql = "SELECT l.id " +
                        "FROM base.langlabel as l " +
                        "JOIN base.langkey AS k ON k.id = l.langKeyId " +
                        "JOIN base.langcode AS c ON c.code = l.langcode " +
                        "WHERE k.Id = @langKeyId " +
                        "AND c.id = @langCodeId ";

            parameters.Add("@langKeyId", dto.LangKeyId);
            parameters.Add("@langCodeId", dto.LangCodeId);
            
            if (dto.Variant == null)
                sql += "AND l.variant IS NULL";
            else
            {
                sql += "AND l.variant = @variant";
                parameters.Add("@variant", dto.Variant);
            }

            try
            {
                using var conn = Sql.GetConnection();
                var result = await conn.QuerySingleOrDefaultAsync<long>(sql, parameters);
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

        public async Task<LangLabelAdminDto> NewLangLabel(LangLabelNewDto dto)
        {
            var sqlCode = "SELECT l.code " +
                        "FROM base.langcode AS l " +
                        "WHERE l.Id = " + dto.LangCodeId;

            var sqlKey = "SELECT k.code " +
                        "FROM base.langkey AS k " +
                        "WHERE k.Id = " + dto.LangKeyId;

            try
            {
                using var conn = Sql.GetConnection();
                
                var langCode = await conn.QuerySingleOrDefaultAsync<string>(sqlCode);
                var langKey = await conn.QuerySingleOrDefaultAsync<string>(sqlKey);

                if (string.IsNullOrEmpty(langCode) || string.IsNullOrEmpty(langKey))
                    return null;

                return new LangLabelAdminDto()
                {
                    Id = GetTempId(),
                    LangCodeId = dto.LangCodeId,
                    LangKeyId = dto.LangKeyId,
                    Variant = dto.Variant,
                    LangKey = langKey,
                };
            }
            catch (Exception ex)
            {
                Log.Logger.Error(
                    ex,
                    "Sql failed: Code:{sqlCode} Key:{sqlKey} ",
                    sqlCode,
                    sqlKey);
                throw;
            }
        }

    }
}
