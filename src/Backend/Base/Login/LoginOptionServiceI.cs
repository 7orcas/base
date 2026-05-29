namespace Backend.Base.Login
{
    public interface LoginOptionServiceI
    {
        Task<LoginOptionEnt> GetLoginOptions(int loginNr);
        Task<LoginOptionDto> InitialiseLoginOptions(LoginOptionEnt ent);
    }
}