namespace Backend.Base.Org
{
    public interface OrgRepoI
    {
        Task<List<OrgEnt>> GetList(bool includeAll);
        Task<OrgEnt?> GetByNr(int nr);
        Task<OrgEnt?> Update(OrgDto Org);
        Task<VersionInfo?> GetVersion(int nr);
        Task<string?> GetCode(int nr);
    }
}
