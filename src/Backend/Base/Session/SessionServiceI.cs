
namespace Backend.Base.Session
{
    public interface SessionServiceI
    {
        Task<SessionEnt> CreateSession(UserAccountEnt userAccount, OrgEnt org, UserConfig config, int sourceApp);
        Task RemoveSession(string key);
        SessionEnt? GetSession(string key);
    }
}
