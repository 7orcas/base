namespace Backend.Base.Login
{
    public interface LoginOptionServiceI
    {
        Task<LoginOptionEnt> GetLoginOptions(string urlSuffix);
        Task<LoginOptionDto> InitialiseLoginOptions(LoginOptionEnt ent);
        Task<LoginOptionEnt> GetLoginOptionDefault(int orgNr);
    }
}