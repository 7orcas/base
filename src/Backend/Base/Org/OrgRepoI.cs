namespace Backend.Base.Org
{
    public interface OrgRepoI
    {
        Task<List<OrgEnt>> GetList();
        Task<OrgEnt?> GetByNr(int nr);
        Task<OrgEnt?> Update(OrgDto Org);
        Task<VersionInfo?> GetVersion(int nr);
    }
}
