
namespace Backend.Base.Login
{
    public interface LoginServiceI
    {
        Task<LoginEnt> LoginUser(string userid, string password, int orgNr, int sourceAppNr, string? langCode);
    }
}
