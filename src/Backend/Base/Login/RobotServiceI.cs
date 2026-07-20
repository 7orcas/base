namespace Backend.Base.Login
{
    public interface RobotServiceI
    {
        Task<(bool success, string message)> Verify(string token, string langCode);
    }
}
