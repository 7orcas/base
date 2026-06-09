using Backend.Base.Token.Ent;

namespace Backend.Base.Token
{
    public interface TokenRepoI
    {
        Task<RefreshToken> SaveRefreshToken(RefreshToken token);
        Task<RefreshToken?> LoadRefreshToken(long id);
        Task RevokeRefreshToken(long id);

    }
}
