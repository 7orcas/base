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
                    "SELECT a.*, z.xxx " +
                    "FROM base.Audit a " +
                    "LEFT JOIN base.userAcc ua ON ua.id = a.userAccId " +
                    "LEFT JOIN base.zzz z ON z.id = ua.zzzId ",
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
                            Created = SqlUtils.GetDateTime(r, "created"),
                            Crud = SqlUtils.GetStringNull(r, "crud"),
                            Details = SqlUtils.GetStringNull(r, "details")
                        });
                    });

            foreach (var a in list)
            { 
                a.EntityType = _entityService.GetEntityTypeName(a.EntityTypeId);
                if (a.UserId == GC.ServiceLoginId) a.User = GC.ServiceAccountName;
            }

            return list;
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

        public void LogInOut(int sourceApp, long orgNr, long userAccId, int entityTypeId)
        {
            Task.Run(async () =>
            {
                try
                {
                    LogAuditRecord(sourceApp, orgNr, userAccId, entityTypeId, null, null, null);
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
            string crud,
            string details)
        {
            LogAuditRecord(session.SourceApp,
                session.Org.Nr,
                session.UserAccount.Id,
                entityTypeId,
                entityId,
                crud,
                details);
        }

        private async void LogAuditRecord(
            int sourceApp,
            long orgNr,
            long userAccId,
            int entityTypeId,
            long? entityId, 
            string crud, 
            string details)
        {
            await Sql.Execute(
                    "INSERT INTO base.Audit " +
                        "(orgNr, source, entityTypeId, entityId, userAccId, crud, details) " +
                    "VALUES (" + 
                        orgNr + "," +
                        sourceApp + "," +
                        entityTypeId + "," +
                        (entityId == null? "null" : entityId) + "," +
                        userAccId + "," +
                        (crud == null ? "null" : "'" + crud + "'") + "," +
                        (details == null ? "null" : "'" + details + "'") + 
                        ")"
            );
        }

    }


}
