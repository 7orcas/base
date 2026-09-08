namespace Backend.Base.Audit
{
    public interface AuditRepoI
    {
        Task<List<AuditEnt>> GetList(AuditSearch search);
        Task<AuditEnt?> GetById(long id);
    }
}
