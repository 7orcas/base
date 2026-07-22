
namespace Backend.Base.Login
{
    public interface LoginServiceI
    {
        Task<LoginEnt> LoginUser(string ipaddress, LoginRequest request, bool mfaValid);
        Task<LoginEnt?> GetLoginById(long id);
        Task<LoginEnt?> GetLoginByEmail(string email);
        Task<bool> ResetRequest(string email, string ipAddress);
        Task<(bool success, string message)> ResetAction(string password, string token, string ipAddress, int orgNr, string langCode);
    }
}
