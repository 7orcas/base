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
            var includeVariant = search.Variant.HasValue;

            var sql = "SELECT DISTINCT c.id, c.code, c.descr AS Description, c.variant, c.flag, c.updated, c.isactive " +
                       "FROM base.langCode AS c " +
                       "WHERE c.id > 0 ";

            if (includeLangCode)
                sql += GetSqlAndClause(search.LangCode, "LangCode", parameters, "c.code", search);

            if (includeVariant)
                sql += GetSqlAndClause(search.Variant.Value, "Variant", parameters, "c.variant");

            sql += " ORDER BY code";
            return await GetList<LangCodeAdmin>(sql, parameters);
        }

        public async Task<List<LangKeyAdmin>> GetLangKeyList(LabelSearch search)
        {
            var parameters = new DynamicParameters();
            var includeLangKey = !string.IsNullOrWhiteSpace(search.LangKey);
            var includeVariant = search.Variant.HasValue;

            var sql = "SELECT DISTINCT k.id, k.code, k.descr AS Description, k.updated " +
                       "FROM base.langKey AS k " +
                       "JOIN base.langLabel AS l ON l.langKeyId = k.Id ";

            if (includeVariant)
                sql += "JOIN base.langCode AS c ON c.Id = l.langCodeId ";

            sql += "WHERE k.id > 0 ";

            if (includeLangKey)
                sql += GetSqlAndClause(search.LangKey, "LangKey", parameters, "k.code", search);

            if (includeVariant)
                sql += GetSqlAndClause(search.Variant.Value, "Variant", parameters, "c.variant");

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
                        "l.langCodeId, " +
                        "l.langKeyId, " +
                        "l.id as Id, " +
                        "c.code AS LangCode, " +
                        "k.code AS LangKey, " +
                        "c.variant, " +
                        "l.code AS Label " +
                       "FROM base.langLabel AS l " +
                           "JOIN base.langCode AS c ON c.Id = l.langCodeId " +
                           "JOIN base.langKey AS k ON k.Id = l.langKeyId " +
                       "WHERE c.id > 0 ";

            sql += GetSqlAndClause(search.LangCodeId, "LangCodeId", parameters, "c.id");
            
            if (includeLangKey)
                sql += GetSqlAndClause(search.LangKey, "LangKey", parameters, "k.code", search);

            if (includeLangLabel)
                sql += GetSqlAndClause(search.LangLabel, "Label", parameters, "l.code", search);

            if (includeVariant)
                sql += GetSqlAndClause(search.Variant.Value, "Variant", parameters, "c.variant");

            sql += " ORDER BY k.code, c.variant ";
            return await GetList<LangLabelAdmin>(sql, parameters);
        }

        public async Task<long> IsExist(LangLabelNewDto dto)
        {
            var parameters = new DynamicParameters();

            var sql = "SELECT l.id " +
                        "FROM base.langlabel as l " +
                        "JOIN base.langkey AS k ON k.id = l.langKeyId " +
                        "JOIN base.langcode AS c ON c.id = l.langCodeId " +
                        "WHERE k.Id = @langKeyId " +
                        "AND c.id = @langCodeId ";

            parameters.Add("@langKeyId", dto.LangKeyId);
            parameters.Add("@langCodeId", dto.LangCodeId);
            
            if (dto.Variant == null)
                sql += "AND c.variant IS NULL";
            else
            {
                sql += "AND c.variant = @variant";
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

        public async Task<LangLabel?> GetLangLabelById(long id)
        {
            try
            {
                return await _context.LangLabels
                    .FirstOrDefaultAsync(x => x.Id == id);

            }
            catch (Exception ex)
            {
                _log.Error($"Error retrieving langLabel with ID {id}", ex);
                return null;
            }
        }

        public async Task<LangLabel?> UpdateLangLabel(LangLabel langLabel, LangLabelAdminDto dto)
        {
            //Cascade delete
            if (dto.IsDelete())
            {
                _context.LangLabels.Remove(langLabel);
                await _context.SaveChangesAsync();
                return null;
            }

            langLabel.Code = dto.LabelEdit;
            langLabel.Updated = DateTimeOffset.UtcNow;
            langLabel.Encode();

            if (dto.IsNew())
                _context.LangLabels.Add(langLabel);

            await _context.SaveChangesAsync();
            return langLabel;
        }

    }
}
