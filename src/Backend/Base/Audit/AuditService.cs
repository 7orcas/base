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


        public AuditDto PopulateDto(SessionEnt session, AuditEnt e)
        {
            var dto = new AuditDto
            {
                Updated = e.Created,
            };

            BaseService.CopyProperties(e, dto);

            var l = session.Labels;

            switch (e.Activity)
            {
                case GC.CrudCreate: dto.ActivityDescr = GetLabel("Crud.C", "Create", l); break;
                case GC.CrudRead: dto.ActivityDescr = GetLabel("Crud.R", "Read", l); break;
                case GC.CrudUpdate: dto.ActivityDescr = GetLabel("Crud.U", "Update", l); break;
                case GC.CrudDelete: dto.ActivityDescr = GetLabel("Crud.D", "Delete", l); break;
                case GC.CrudReadList: dto.ActivityDescr = GetLabel("Crud.L", "Read List", l); break;
                case GC.AuditLogin: dto.ActivityDescr = GetLabel("Login", "Login", l); break;
                case GC.AuditLogout: dto.ActivityDescr = GetLabel("Logout", "Logout", l); break;
                default: dto.ActivityDescr = ""; break;
            }

            return dto;
        }

        public void LogAction(SessionEnt session, int entityTypeNr, string action, AuditInfo info)
        {
            Task.Run(async () =>
            {
                try
                {
                    LogAuditRecord(session, entityTypeNr, action, info.Id, info.Code, info.Version, info.Json);
                }
                catch (Exception ex)
                {
                    _log.Error("Audit Read:" + ex.Message);
                }
            });
        }

        public void LogIn(SessionEnt session)
        {
            Task.Run(async () =>
            {
                try
                {
                    LogAuditRecord(session, GC.EntityTypeLogin, GC.AuditLogin, null, null, null, null);
                }
                catch (Exception ex)
                {
                    _log.Error("Audit Read:" + ex.Message);
                }
            });
        }

        public void LogOut(SessionEnt session)
        {
            Task.Run(async () =>
            {
                try
                {
                    LogAuditRecord(session, GC.EntityTypeLogout, GC.AuditLogout, null, null, null, null);
                }
                catch (Exception ex)
                {
                    _log.Error("Audit Read:" + ex.Message);
                }
            });
        }

        private async void LogAuditRecord(SessionEnt session,
            int entityTypeNr,
            string? activity,
            long? entityId,
            string? entityCode,
            int? entityVersion,
            string? details)
        {
            await _auditRepo.LogAuditRecord(
                session.SourceApp,
                session.OrgNr,
                session.UserAccount.Id,
                session.MasqueradeId,
                entityTypeNr,
                entityId,
                entityCode,
                entityVersion,
                activity,
                details);
        }

        private string GetLabel(string langKey, string nullDefault, Dictionary<string, string> labels)
        {
            if (labels.ContainsKey(langKey))
                return labels[langKey];
            return nullDefault;
        }
    }


}
