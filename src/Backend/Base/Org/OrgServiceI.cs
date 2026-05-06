
namespace Backend.Base.Org
{
    public interface OrgServiceI
    {
        Task<OrgEnt> GetOrg(int id);
        Task<List<OrgEnt>> GetOrgList();
        Task UpdateOrg(OrgEnt org);
    }
}
