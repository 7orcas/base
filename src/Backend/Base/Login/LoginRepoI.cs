namespace Backend.Base.Login
{
    public interface LoginRepoI
    {
        Task<bool> IsUniqueUsername(string username);
        Task<bool> IsUniqueEmail(string email);
        Task<LoginEnt?> GetLoginByUsername(string userid);
        Task<LoginEnt?> GetLoginByEmail(string email);
        Task<LoginEnt?> GetLoginById(long id);
        Task<UserAccountEnt?> GetAccount(long loginId, int orgNr);
        Task<bool> SetAttempts(long id, int attempts);
        Task<bool> SetLockoutTimestamp(long id, DateTimeOffset lockout);
        Task UpdateLastLogin(long id, long accountId);

        Task<bool> CreateSignup(LoginEnt login);
        Task<bool> VerifySignup(LoginEnt login);
        Task<bool> SetMfaKey(long id, string key);
        Task<bool> EnableMfa(long id);
    }
}
