
namespace Backend.Base.Audit
{
    public interface AuditServiceI
    {
        void LogAction(SessionEnt session, int entityTypeNr, long? entityId, string crudAction, string details);
        void LogInOut(SessionEnt session, int entityTypeNr);
        Task<List<AuditList>> GetEvents(SessionEnt session);
        AuditDto Load(AuditList e);
    }
}
