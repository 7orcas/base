
namespace Backend.Base.Org
{
    public interface OrgServiceI
    {
        Task<OrgEnt> GetOrg(int id);
        Task<List<OrgEnt>> GetOrgList();
        Task UpdateOrg(OrgEnt org);
        OrgDto Populate(OrgEnt org);
        bool ValidatePassword(string pw, OrgEnt org);
        Task<string> GetPasswordRules(string langCode, int orgNr);
    }
}
