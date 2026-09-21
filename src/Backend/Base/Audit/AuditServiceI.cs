
namespace Backend.Base.Audit
{
    public interface AuditServiceI
    {
        void LogAction(SessionEnt session, int entityTypeNr, long? entityId, int? entityVersion, string crudAction, string details);
        void LogIn(SessionEnt session);
        void LogOut(SessionEnt session);
        Task<List<AuditEnt>> GetEvents(SessionEnt session, AuditSearch search);
        Task<AuditEnt?> GetById(SessionEnt session, long id);
        AuditDto Populate(SessionEnt session, AuditEnt e);
        void ConfigureSearch(SessionEnt session, AuditSearch search);
    }
}
