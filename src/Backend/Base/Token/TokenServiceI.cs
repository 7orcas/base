
using Backend.Base.Token.Ent;

namespace Backend.Base.Token
{
    public interface TokenServiceI
    {
        public string CreateJWToken(TokenValues tokenValues);
        public string CreateResetRequestToken(TokenValues tokenValues);
        string? GetJWToken(string key);
        TokenValues DecodeToken(string token);

        Task<(string jwToken, RefreshToken refreshToken)> RefreshToken(string refreshTokenString, string ipAddress, string revokedBy);
        Task<RefreshToken> CreateRefreshToken(TokenValues tv);
    }
}
