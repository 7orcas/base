
using Backend.Base.Token.Ent;

namespace Backend.Base.Token
{
    public interface TokenServiceI
    {
        public string CreateToken(TokenValues tokenValues);
        string? GetToken(string key);
        TokenValues DecodeToken(string token);
        
        
        RefreshToken CreateRefreshToken(TokenValues tv);
        RefreshToken? GetRefreshToken(string tokenKey);
    }
}
