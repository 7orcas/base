
using Org.BouncyCastle.Tls;

namespace Backend.Base.Session
{
    public interface SessionServiceI
    {
        Task<TSession> CreateSession<TSession>(LoginAccountEnt userAccount, OrgEnt org, ConfigUser config, long? masqueradeId, int sourceApp, string ipAddress, string? sessionKey)
            where TSession : SessionEnt, new();
        Task RemoveSession(string key);
        SessionEnt? GetSession(string key);
    }
}
