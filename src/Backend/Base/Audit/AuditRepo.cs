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

        public async Task<List<AuditEnt>> GetList(AuditSearch search, int orgNr)
        {
            return await GetList(null, search, orgNr);
        }

        public async Task<AuditEnt?> GetById(long id)
        {
            var result = await GetList(id, null, null);
            return result.FirstOrDefault();
        }

        private async Task<List<AuditEnt>> GetList(long? id, AuditSearch? search, int? orgNr)
        {
            var details = search != null ? "" : "a.details,";
            var sql =
                @"SELECT a.id,
                        a.orgNr,
                        a.source,
                        a.entityTypeNr,
                        a.entityId,
                        a.userAccId,
                        a.masqueradeId,
                        a.created,
                        a.crud,"
                        + details +
                      @"z.xxx AS userName, 
                        m.xxx AS masquerade
                  FROM base.Audit a
                  LEFT JOIN base.userAcc ua ON ua.id = a.userAccId
                  LEFT JOIN base.zzz z ON z.id = ua.zzzId
                  LEFT JOIN base.zzz m ON m.id = a.masqueradeId
                  WHERE 1 = 1";

            var parameters = new DynamicParameters();


            if (id.HasValue)
            {
                sql += " AND a.id = @Id";
                parameters.Add("Id", id.Value);
            }

            if (search != null)
            {
                sql += " AND a.orgNr = @OrgNr";
                parameters.Add("OrgNr", orgNr.Value);
                                
                if (search.OrgNr.HasValue)
                {
                    sql += " AND a.orgNr = @OrgNr";
                    parameters.Add("OrgNr", search.OrgNr.Value);
                }

                if (search.Source.HasValue)
                {
                    sql += " AND a.source = @Source";
                    parameters.Add("Source", search.Source.Value);
                }

                if (search.EntityId.HasValue)
                {
                    sql += " AND a.entityId = @EntityId";
                    parameters.Add("EntityId", search.EntityId.Value);
                }

                if (!string.IsNullOrWhiteSpace(search.Username))
                {
                    sql += " AND " + SqlUnaccent("z.xxx", "Username", search);
                    parameters.Add("Username", SqlParameter(search.Username, search));
                }

                //if (search.EntityTypeNr.HasValue)
                //{
                //    sql += " AND a.entityTypeNr = @EntityTypeNr";
                //    parameters.Add("EntityTypeNr", search.EntityTypeNr.Value);
                //}

                if (!string.IsNullOrEmpty(search.CRUD))
                {
                    var crudParams = new List<string>();

                    for (var i = 0; i < search.CRUD.Length; i++)
                    {
                        var paramName = $"Crud{i}";

                        crudParams.Add($"@{paramName}");
                        parameters.Add(paramName, search.CRUD[i].ToString());
                    }

                    sql += $" AND a.crud IN ({string.Join(",", crudParams)})";
                }
    
                sql += " ORDER BY a.created " + GetSqlLimitClauseForSearch(search);
            }

            return await GetList<AuditEnt>(sql, parameters);
        }

        public async Task LogAuditRecord(
            int sourceApp,
            long orgNr,
            long userAccId,
            long? masqueradeId,
            int entityTypeNr,
            long? entityId,
            string crud,
            string details)
        {
            await Sql.ExecuteAsync(
                    "INSERT INTO base.Audit " +
                        "(orgNr, source, entityTypeNr, entityId, userAccId, masqueradeId, crud, details) " +
                    "VALUES (" +
                        orgNr + "," +
                        sourceApp + "," +
                        entityTypeNr + "," +
                        (entityId == null ? "null" : entityId) + "," +
                        userAccId + "," +
                        (masqueradeId == null ? "null" : masqueradeId) + "," +
                        (crud == null ? "null" : "'" + crud + "'") + "," +
                        (details == null ? "null" : "'" + details + "'") +
                        ")"
            );
        }

    }
}
