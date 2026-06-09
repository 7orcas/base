
using Backend.Base.Token.Ent;

namespace Backend.Base.Token
{
    public interface TokenServiceI
    {
        public string CreateJWToken(TokenValues tokenValues);
        string? GetJWToken(string key);
        TokenValues DecodeJWToken(string token);


        Task<RefreshToken> CreateRefreshToken(TokenValues tv);
        Task<RefreshToken?> GetRefreshToken(string tokenKey);
    }
}
