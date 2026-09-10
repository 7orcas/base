namespace Backend.Base.Audit
{
    public interface AuditRepoI
    {
        Task<List<AuditEnt>> GetList(AuditSearch search);
        Task<AuditEnt?> GetById(long id);
        Task LogAuditRecord(
            int sourceApp,
            long orgNr,
            long userAccId,
            long? masqueradeId,
            int entityTypeNr,
            long? entityId,
            string crud,
            string details);
    }
}
