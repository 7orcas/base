namespace Backend.Base.Version
{
    public interface VersionRepoI
    {
        Task<VersionInfo?> GetVersion(long id);
    }
}
