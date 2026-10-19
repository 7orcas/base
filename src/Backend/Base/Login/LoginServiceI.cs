
namespace Backend.Base.Login
{
    public interface LoginServiceI
    {
        Task<(LoginEnt Login, TSession Session)> LoginUser<TSession>(string ipaddress, LoginRequest request, bool mfaValid)
            where TSession : SessionEnt, new();
        Task<(LoginResponseDto res, LoginSuccessDto? dto)> ConfigureLoginResponse(LoginRequest request, LoginEnt login);
        Task<LoginSuccessDto> ConfigureLoginResponseMfa(LoginRequest request, LoginEnt login);
        Task<LoginEnt?> GetLoginById(long id);
        Task<LoginEnt?> GetLoginByEmail(string email);
        string PasswordHash(string password);
        Task<(bool success, string message)> ResetRequest(string email, string ipAddress, string langCode);
        Task<(bool success, string message)> ResetAction(string password, string token, string ipAddress, int orgNr, string langCode);
    }
}
