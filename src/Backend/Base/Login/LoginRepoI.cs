namespace Backend.Base.Login
{
    public interface LoginRepoI
    {
        Task<bool> IsUniqueUsername(string username);
        Task<bool> IsUniqueEmail(string email);
        Task<LoginEnt?> GetLoginByUsername(string userid);
        Task<LoginEnt?> GetLoginByEmail(string email);
        Task<LoginEnt?> GetLoginById(long id);
        Task<bool> CreateSignup(LoginEnt login);
        Task<bool> UpdateSignup(LoginEnt login);
    }
}
