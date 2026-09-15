
namespace Backend.Base.Audit
{
    public interface AuditServiceI
    {
        void LogAction(SessionEnt session, int entityTypeNr, long? entityId, string crudAction, string details);
        void LogInOut(SessionEnt session, int entityTypeNr);
        Task<List<AuditEnt>> GetEvents(SessionEnt session, AuditSearch search);
        Task<AuditEnt?> GetById(SessionEnt session, long id);
        AuditDto Populate(AuditEnt e);
        void ConfigureSearch(SessionEnt session, AuditSearch search);
    }
}
