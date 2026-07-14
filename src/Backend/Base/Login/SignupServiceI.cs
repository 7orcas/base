namespace Backend.Base.Login
{
    public interface SignupServiceI
    {
        Task<(bool success, string message)> SignupUser(string ipaddress, string username, string email, string password, int orgNr, string langCode);
        Task<(bool valid, string message)> VerifyEmail(string ipAddress, string email, int orgNr, string langCode);
    }
}
