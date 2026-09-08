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
        private readonly AuditRepoI _auditRepo;

        public AuditService(EntityServiceI entityService,
            AuditRepoI auditRepo)
        {
            _log = Log.Logger;
            _entityService = entityService;
            _auditRepo = auditRepo;
        }

        public async Task<List<AuditList>> GetEvents(SessionEnt session, AuditSearch search)
        {
            var list = await _auditRepo.GetList(search);
            var listX = new List<AuditList>();
            foreach (var ent in list)
                listX.Add(Populate(ent));

            return listX;
        }

        public async Task<AuditList?> GetById(long id)
        {
            var ent = await _auditRepo.GetById(id);
            if (ent == null) return null;
            return Populate(ent);
        }

        private AuditList Populate(AuditEnt ent)
        {
            var list = new AuditList();
            BaseService.CopyProperties(ent, list);

            list.EntityType = _entityService.GetEntityTypeName(list.EntityTypeNr);
            if (list.UserAccId == GC.ServiceLoginId) list.User = GC.ServiceAccountName;
            if (list.MasqueradeId != null && list.MasqueradeId == GC.ServiceLoginId) list.Masquerade = GC.ServiceAccountName;
            return list;
        }


        public AuditDto Populate(AuditList e)
        {
            var dto = new AuditDto
            {
                Id = e.Id,
                OrgNr = e.OrgNr,
                Source = e.Source,
                EntityType = e.EntityType,
                EntityId = e.EntityId,
                User = e.User + (string.IsNullOrEmpty(e.Masquerade) ? "" : " (" + e.Masquerade + ")"),
                Updated = e.Created,
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

        public void LogAction(SessionEnt session, int entityTypeNr, long? entityId, string crudAction, string details)
        {
            Task.Run(async () =>
            {
                try
                {
                    LogAuditRecord(session, entityTypeNr, entityId, crudAction, details);
                }
                catch (Exception ex)
                {
                    _log.Error("Audit Read:" + ex.Message);
                }
            });
        }

        public void LogInOut(SessionEnt session, int entityTypeNr)
        {
            Task.Run(async () =>
            {
                try
                {
                    LogAuditRecord(session, entityTypeNr, null, null, null);
                }
                catch (Exception ex)
                {
                    _log.Error("Audit Read:" + ex.Message);
                }
            });
        }

        private async void LogAuditRecord(SessionEnt session,
            int entityTypeNr,
            long? entityId,
            string? crud,
            string? details)
        {
            LogAuditRecord(session.SourceApp,
                session.OrgNr,
                session.UserAccount.Id,
                session.MasqueradeId,
                entityTypeNr,
                entityId,
                crud,
                details);
        }

        private async void LogAuditRecord(
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
