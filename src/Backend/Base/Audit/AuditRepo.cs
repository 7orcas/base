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
                        a.entityVersion,
                        a.userAccId,
                        a.masqueradeId,
                        a.created,
                        a.activity,"
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

                if (search.FromDate.HasValue)
                {
                    sql += " AND a.created >= @CreatedFrom";
                    parameters.Add("CreatedFrom", search.FromDate.Value);
                }

                if (search.ToDate.HasValue)
                {
                    sql += " AND a.created <= @CreatedTo";
                    parameters.Add("CreatedTo", search.ToDate.Value);
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

                if (search.EntityTypeNrs != null)
                {
                    if (search.EntityTypeNrs.Count == 0)
                        sql += " AND 1 = 0"; // No entity types specified, return no results
                    else
                    {
                        var entityTypeNrParams = new List<string>();
                        for (var i = 0; i < search.EntityTypeNrs.Count; i++)
                        {
                            var paramName = $"EntityTypeNr{i}";
                            entityTypeNrParams.Add($"@{paramName}");
                            parameters.Add(paramName, search.EntityTypeNrs[i]);
                        }
                        sql += $" AND a.entityTypeNr IN ({string.Join(",", entityTypeNrParams)})";
                    }
                }

                if (!string.IsNullOrEmpty(search.CRUD))
                {
                    var crudParams = new List<string>();

                    for (var i = 0; i < search.CRUD.Length; i++)
                    {
                        var value = search.CRUD[i].ToString();
                        if (value == ",") continue;
                        var paramName = $"Crud{i}";

                        crudParams.Add($"@{paramName}");
                        parameters.Add(paramName, value.ToLower());
                    }

                    sql += $" AND a.activity IN ({string.Join(",", crudParams)})";
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
            int? entityVersion,
            string activity,
            string details)
        {
            await Sql.ExecuteAsync(
                    "INSERT INTO base.Audit " +
                        "(orgNr, source, entityTypeNr, entityId, entityVersion, userAccId, masqueradeId, activity, details) " +
                    "VALUES (" +
                        orgNr + "," +
                        sourceApp + "," +
                        entityTypeNr + "," +
                        (entityId == null ? "null" : entityId) + "," +
                        (entityVersion == null ? "null" : entityVersion) + "," +
                        userAccId + "," +
                        (masqueradeId == null ? "null" : masqueradeId) + "," +
                        (activity == null ? "null" : "'" + activity + "'") + "," +
                        (details == null ? "null" : "'" + details + "'") +
                        ")"
            );
        }

    }
}
