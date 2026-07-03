
namespace Backend.Base.Org
{
    public interface OrgServiceI
    {
        Task<OrgEnt> GetOrg(int id);
        Task<List<OrgEnt>> GetOrgList();
        Task UpdateOrg(OrgEnt org);
        OrgDto Populate(OrgEnt org);
        (bool valid, string message) ValidatePassword(string pw, OrgEnt org, Dictionary<string, string>? labels);
        Task<string> GetPasswordRules(string langCode, int orgNr);
    }
}
