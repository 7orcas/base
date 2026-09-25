
namespace Backend.Base.Audit
{
    public interface AuditServiceI
    {
        void LogAction(SessionEnt session, int entityTypeNr, string action, AuditInfo info);
        void LogIn(SessionEnt session);
        void LogOut(SessionEnt session);
        Task<List<AuditEnt>> GetEvents(SessionEnt session, AuditSearch search);
        Task<AuditEnt?> GetById(SessionEnt session, long id);
        AuditDto PopulateDto(SessionEnt session, AuditEnt e);
        void ConfigureSearch(SessionEnt session, AuditSearch search);
    }
}
