
namespace Backend.Base.Audit
{
    public interface AuditServiceI
    {
        void ReadList(SessionEnt session, int entityTypeId, string query);
        void ReadEntity(SessionEnt session, int entityTypeId, long entityId);
        void LogInOut(SessionEnt session, int entityTypeId);
        Task<List<AuditList>> GetEvents(SessionEnt session);
        AuditDto Load(AuditList e);
    }
}
