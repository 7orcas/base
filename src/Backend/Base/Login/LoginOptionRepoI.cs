namespace Backend.Base.Login
{
    public interface LoginOptionRepoI
    {
        Task<LoginOptionEnt?> GetLoginOptions(string urlSuffix);
        Task<LoginOptionEnt?> GetLoginOptions(int orgNr);
    }
}
