
using Backend.Base.Token.Ent;

namespace Backend.Base.Token
{
    public interface TokenServiceI
    {
        string CreateToken(TokenValues tokenValues);
        TokenValues DecodeToken(string token);
        void AddToken(string key, string token);
        string? GetToken(string key);
        
    }
}
