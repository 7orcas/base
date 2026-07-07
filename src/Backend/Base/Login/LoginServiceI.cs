
namespace Backend.Base.Login
{
    public interface LoginServiceI
    {
        Task<LoginEnt> LoginUser(string ipaddress, string userid, string password, int orgNr, int sourceAppNr, string langCode, bool mfaValid);
        Task<LoginEnt?> GetLoginById(long id);
        Task<LoginEnt?> GetLoginByEmail(string email);
        Task<bool> SetMfaKey(long id, string key);
        Task<bool> EnableMfa(long id);
        Task<bool> ResetRequest(string email, string ipAddress);
        Task<(bool success, string message)> ResetAction(string password, string token, string ipAddress, int orgNr, string langCode);
    }
}
