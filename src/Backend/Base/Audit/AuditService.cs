using DocumentFormat.OpenXml.Spreadsheet;
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


        public void ConfigureSearch(SessionEnt session, AuditSearch search)
        {
            if (search.FromDate.HasValue)
            {
                search.FromDate = search.FromDate.Value + TimeSpan.Zero;
                if (search.FromTime.HasValue)
                    search.FromDate = search.FromDate.Value + search.FromTime.Value;
            }

            if (search.ToDate.HasValue)
                search.ToDate = search.ToDate.Value.AddDays(1) + TimeSpan.Zero;

            if (!string.IsNullOrWhiteSpace(search.EntityType))
                search.EntityTypeNrs = _entityService.GetEntityTypeNrs(session, search.EntityType, search.TextFieldSearchType);

        }

        public async Task<List<AuditEnt>> GetEvents(SessionEnt session, AuditSearch search)
        {
            var list = await _auditRepo.GetList(search, session.OrgNr);
            foreach (var ent in list)
                PopulateDecriptions(session, ent);
            return list;
        }

        public async Task<AuditEnt?> GetById(SessionEnt session, long id)
        {
            var ent = await _auditRepo.GetById(id);
            if (ent == null) return null;
            return PopulateDecriptions(session, ent);
        }

        private AuditEnt PopulateDecriptions(SessionEnt session, AuditEnt ent)
        {
            ent.EntityType = _entityService.GetEntityTypeName(session, ent.EntityTypeNr);
            if (ent.UserAccId == GC.ServiceLoginId) ent.UserName = GC.ServiceAccountName;
            if (ent.MasqueradeId != null && ent.MasqueradeId == GC.ServiceLoginId) ent.Masquerade = GC.ServiceAccountName;
            return ent;
        }


        public AuditDto Populate(AuditEnt e)
        {
            var dto = new AuditDto
            {
                Id = e.Id,
                OrgNr = e.OrgNr,
                Source = e.Source,
                EntityTypeNr = e.EntityTypeNr,
                EntityType = e.EntityType,
                EntityId = e.EntityId,
                UserName = e.UserName,
                Masquerade = e.Masquerade,
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
            await _auditRepo.LogAuditRecord(
                session.SourceApp,
                session.OrgNr,
                session.UserAccount.Id,
                session.MasqueradeId,
                entityTypeNr,
                entityId,
                crud,
                details);
        }
        

    }


}
