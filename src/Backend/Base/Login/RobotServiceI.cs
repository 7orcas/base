namespace Backend.Base.Login
{
    public interface RobotServiceI
    {
        Task<(bool success, string temptoken, string message)> Verify(string ipAddress, string token, string langCode);
    }
}
