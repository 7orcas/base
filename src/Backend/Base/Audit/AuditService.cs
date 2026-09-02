using GC = Backend.GlobalConstants;

/// <summary>
/// Audit of:
/// - all transactions in db
/// - logins / outs events
/// - no permission events
/// Created: June 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Audit
{
    public class AuditService: AuditServiceI
    {
        protected readonly Serilog.ILogger _log;
        private readonly EntityServiceI _entityService;

        public AuditService(EntityServiceI entityService) 
        {
            _log = Log.Logger;
            _entityService = entityService;
        }

        public async Task<List<AuditList>> GetEvents(SessionEnt session)
        {
            List<AuditList> list = new List<AuditList>();
            await Sql.Run(
                    "SELECT a.*, z.xxx, m.xxx AS masquerade " +
                    "FROM base.Audit a " +
                    "LEFT JOIN base.userAcc ua ON ua.id = a.userAccId " +
                    "LEFT JOIN base.zzz z ON z.id = ua.zzzId " +
                    "LEFT JOIN base.zzz m ON m.id = a.masqueradeId ",
                    r => {
                        list.Add(new AuditList()
                        {
                            Id = SqlUtils.GetId(r),
                            orgNr = SqlUtils.GetOrgNr(r),
                            Source = SqlUtils.GetInt(r, "source"),
                            EntityTypeId = SqlUtils.GetInt(r, "entityTypeId"),
                            EntityId = SqlUtils.GetIntNull(r, "entityId"),
                            UserId = SqlUtils.GetId(r, "userAccId"),
                            User = SqlUtils.GetStringNull(r, "xxx"),
                            MasqueradeId = SqlUtils.GetIdNull(r, "masqueradeId"),
                            Masquerade = SqlUtils.GetStringNull(r, "masquerade"),
                            Created = SqlUtils.GetDateTime(r, "created"),
                            Crud = SqlUtils.GetStringNull(r, "crud"),
                            Details = SqlUtils.GetStringNull(r, "details")
                        });
                    });

            foreach (var a in list)
            { 
                a.EntityType = _entityService.GetEntityTypeName(a.EntityTypeId);
                if (a.UserId == GC.ServiceLoginId) a.User = GC.ServiceAccountName;
                if (a.MasqueradeId != null && a.MasqueradeId == GC.ServiceLoginId) a.Masquerade = GC.ServiceAccountName;
            }

            return list;
        }

        public AuditDto Load(AuditList e)
        {
            var dto = new AuditDto
            {
                Id = e.Id,
                orgNr = e.orgNr,
                Source = e.Source,
                EntityType = e.EntityType,
                EntityId = e.EntityId,
                User = e.User + (string.IsNullOrEmpty(e.Masquerade) ? "" : " (" + e.Masquerade + ")"),
                Created = e.Created,
                Details = e.Details,
            };

            switch (e.Crud)
            {
                case GC.CrudCreate: dto.Crud = "Create"; break;
                case GC.CrudRead: dto.Crud = "Read"; break;
                case GC.CrudUpdate: dto.Crud = "Update"; break;
                case GC.CrudDelete: dto.Crud = "Delete"; break;
                case GC.CrudReadList: dto.Crud = "Read List"; break;
                case GC.CrudIgnore: dto.Crud = ""; break;
                default: dto.Crud = ""; break;
            }

            return dto;
        }


        public void ReadEntity(SessionEnt session, int entityTypeId, long entityId)
        {
            Task.Run(async () =>
            {
                try
                {
                    LogAuditRecord(session, entityTypeId, entityId, GC.CrudRead, null);
                }
                catch (Exception ex) 
                { 
                    _log.Error("Audit Read:" + ex.Message);
                }
            });
        }

        public void ReadList(SessionEnt session, int entityTypeId, string query)
        {
            Task.Run(async () =>
            {
                try
                {
                    LogAuditRecord(session, entityTypeId, null, GC.CrudReadList, query);
                }
                catch (Exception ex)
                {
                    _log.Error("Audit Read:" + ex.Message);
                }
            });
        }

        public void LogInOut(SessionEnt session, int entityTypeId)
        {
            Task.Run(async () =>
            {
                try
                {
                    LogAuditRecord(session, entityTypeId, null, null, null);
                }
                catch (Exception ex)
                {
                    _log.Error("Audit Read:" + ex.Message);
                }
            });
        }

        private async void LogAuditRecord(SessionEnt session,
            int entityTypeId,
            long? entityId,
            string? crud,
            string? details)
        {
            LogAuditRecord(session.SourceApp,
                session.OrgNr,
                session.UserAccount.Id,
                session.MasqueradeId,
                entityTypeId,
                entityId,
                crud,
                details);
        }

        private async void LogAuditRecord(
            int sourceApp,
            long orgNr,
            long userAccId,
            long? masqueradeId,
            int entityTypeId,
            long? entityId, 
            string crud, 
            string details)
        {
            await Sql.ExecuteAsync(
                    "INSERT INTO base.Audit " +
                        "(orgNr, source, entityTypeId, entityId, userAccId, masqueradeId, crud, details) " +
                    "VALUES (" + 
                        orgNr + "," +
                        sourceApp + "," +
                        entityTypeId + "," +
                        (entityId == null? "null" : entityId) + "," +
                        userAccId + "," +
                        (masqueradeId == null ? "null" : masqueradeId) + "," +
                        (crud == null ? "null" : "'" + crud + "'") + "," +
                        (details == null ? "null" : "'" + details + "'") + 
                        ")"
            );
        }

    }


}
