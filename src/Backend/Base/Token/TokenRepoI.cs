using Backend.Base.Token.Ent;

namespace Backend.Base.Token
{
    public interface TokenRepoI
    {
        Task<RefreshToken?> LoadRefreshToken(string key);
        Task SaveRefreshToken(RefreshToken token);
        Task RevokeRefreshToken(RefreshToken token);

    }
}
