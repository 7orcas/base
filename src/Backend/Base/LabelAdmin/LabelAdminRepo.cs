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


        public async Task<List<LangCodeAdmin>> GetLangCodeList()
        {
            var sql = "SELECT DISTINCT " +
                "id, code, descr AS Description, flag, updated, isactive "
                       + "FROM base.langCode ";

            sql += " ORDER BY code";

            return await GetList<LangCodeAdmin>(sql);
        }

        public async Task<List<LangKeyAdmin>> GetLangKeyList()
        {
            var sql = "SELECT DISTINCT " +
                "id, code, descr AS Description, updated, version "
                       + "FROM base.langKey ";

            sql += " ORDER BY code";

            return await GetList<LangKeyAdmin>(sql);
        }


        public async Task<List<LangLabelAdmin>> GetLangLabelList(string langCode)
        {
            var sql = "SELECT DISTINCT " +
                        "c.id as LangCodeId, " +
                        "l.langKeyId, " +
                        "l.id as Id, " +
                        "l.langCode, " +
                        "k.code AS key, " +
                        "l.variant, " +
                        "l.code, " +
                        "l.tooltip " +
                       "FROM base.langLabel AS l " +
                           "JOIN base.langKey AS k ON k.Id = l.langKeyId " +
                           "JOIN base.langCode AS c ON c.code = l.langCode " +
                       "WHERE l.langCode = @langCode ";

            sql += " ORDER BY code";

            var parameters = new DynamicParameters();
            parameters.Add("langCode", langCode);

            return await GetList<LangLabelAdmin>(sql, parameters);
        }

    }
}
