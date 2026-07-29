
namespace Backend.Base.Session
{
    public interface SessionServiceI
    {
        Task<SessionEnt> CreateSession(UserAccountEnt userAccount, OrgEnt org, UserConfig config, long? masqueradeId, int sourceApp, string ipAddress);
        Task RemoveSession(string key);
        SessionEnt? GetSession(string key);
    }
}
