
namespace Backend.Base.Login
{
    public interface LoginServiceI
    {
        Task<LoginEnt> LoginUser(string ipaddress, string userid, string password, int orgNr, int sourceAppNr, string? langCode);
        Task<LoginEnt?> GetLogin(long id);
        Task<bool> SetMfaKey(long id, string key);
        Task<bool> EnableMfa(long id);
    }
}
