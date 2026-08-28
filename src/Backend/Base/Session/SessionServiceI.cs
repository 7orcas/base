
namespace Backend.Base.Session
{
    public interface SessionServiceI
    {
        Task<SessionEnt> CreateSession(LoginAccountEnt userAccount, OrgEnt org, ConfigUser config, long? masqueradeId, int sourceApp, string ipAddress);
        Task RemoveSession(string key);
        SessionEnt? GetSession(string key);
    }
}
