
namespace Backend.Base.Org
{
    public interface OrgServiceI
    {
        Task<OrgEnt> GetOrg(int nr);
        Task<List<OrgEnt>> GetOrgList();
        Task<OrgEnt?> UpdateOrg(SessionEnt session, OrgDto orgDto);
        OrgDto PopulateList(SessionEnt session, OrgEnt org);
        OrgDto Populate(SessionEnt session, OrgEnt org);
        Task<DefinitionDto> GetDefinition(SessionEnt session);
        Task<List<ValidationDto>> ValidateOrg(SessionEnt session, List<OrgDto> update);
        (bool valid, string message) ValidatePassword(string pw, OrgEnt org, Dictionary<string, string>? labels);
        Task<string> GetPasswordRules(string langCode, int orgNr);
    }
}
