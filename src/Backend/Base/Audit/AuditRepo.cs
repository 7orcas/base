
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using GC = Backend.GlobalConstants;

namespace Backend.Base.Audit
{
    /// <summary>
    /// Audit database records
    /// </summary>
    /// <author>John Stewart</author>
    /// <created>September, 2026</created>
    /// <license>**Licence**</license>
    public class AuditRepo : BaseRepo, AuditRepoI
    {

        public AuditRepo(IServiceProvider serviceProvider)
            : base(serviceProvider) { }

        public async Task<List<AuditEnt>> GetList(AuditSearch search)
        {
            return await GetList(null, search);
        }

        public async Task<AuditEnt?> GetById(long id)
        {
            var result = await GetList(id, null);
            return result.FirstOrDefault();
        }


        private async Task<List<AuditEnt>> GetList(long? id, AuditSearch? search)
        {
            var sql = "SELECT a.*, z.xxx AS user, m.xxx AS masquerade " +
                    "FROM base.Audit a " +
                    "LEFT JOIN base.userAcc ua ON ua.id = a.userAccId " +
                    "LEFT JOIN base.zzz z ON z.id = ua.zzzId " +
                    "LEFT JOIN base.zzz m ON m.id = a.masqueradeId ";

            if (id.HasValue)
            {
                sql += " WHERE a.id = " + id.Value;
            }
            else
            {
                if (!string.IsNullOrEmpty(search.Username))
                {
                    sql += " AND user LIKE '%" + search.Username + "%'";
                }

                sql += " ORDER BY a.created "
                    + GetSqlLimitClauseForSearch(search);
            }

            return await GetList<AuditEnt>(sql);
        }

    }
}
