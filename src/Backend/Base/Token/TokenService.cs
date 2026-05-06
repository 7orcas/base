using GC = Backend.GlobalConstants;
using Backend.Base.Token.Ent;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

/// <summary>
/// Tokens are used to control authorisation to this app
/// Created: March 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Token
{
    public class TokenService: BaseService, TokenServiceI
    {
        private readonly IMemoryCache _memoryCache;
        private const int PAD_TOKEN = 5;

        public TokenService(IServiceProvider serviceProvider,
            IMemoryCache memoryCache)
            : base(serviceProvider)
        {
            _memoryCache = memoryCache;
        }


        /// <summary>
        /// Login process creates this token
        /// </summary>
        /// <param name="tv"></param>
        /// <returns></returns>
        public string CreateToken(TokenValues tv)
        {
            var claims = new[]
            {
                new Claim("Key", tv.SessionKey),
                new Claim("Org", "" + tv.Org),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenParameters._Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: TokenParameters._Issuer,
                audience: TokenParameters._Audience,
                claims: claims,
                expires: DateTime.Now.AddHours(30),
                signingCredentials: creds
                );

            _log.Debug("Create token, TokenValues=" + tv.ToLogString());

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Every call will use this method
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public TokenValues DecodeToken(string token)
        {
            token = token.Trim();
            //token = token.Substring(TOKEN_PREFIX.Length);
            var tokenHandler = new JwtSecurityTokenHandler();

            TokenValues tv = new TokenValues();
            try
            {
                // Validate token and decode
                var principal = tokenHandler.ValidateToken(token, TokenParameters.GetParameters(), out var validatedToken);

                // Extract claims
                tv.SessionKey = principal.FindFirst("Key")?.Value.ToString();
                int.TryParse(principal.FindFirst("Org")?.Value, out int orgNr);
                tv.Org = orgNr;
                _log.Debug("Decode token, SessionKey=" + tv.SessionKey);
                return tv;
            }
            catch (Exception ex)
            {
                _log.Error("Token validation failed: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Login process adds this token
        /// </summary>
        /// <param name="key"></param>
        /// <param name="token"></param>
        public void AddToken(string key, string token)
        {
            string tokenX = AppSettings.MaxGetTokenCalls.ToString().PadLeft(PAD_TOKEN, '0') + token;
            _log.Debug("Add token, key=" + key + ", token=" + tokenX);

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(AppSettings.CacheExpirationAddSeconds) // Cache expiration
            };
            _memoryCache.Set(Key(key), tokenX, cacheEntryOptions);
        }

        /// <summary>
        /// Login process can call this method twice
        /// The number of calls is limited by appsetting value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string? GetToken(string key)
        {
            _log.Debug("GetToken, key=" + key);

            if (_memoryCache.TryGetValue(Key(key), out var cachedValue))
            {
                var tokenX = cachedValue.ToString();
                var calls = int.Parse(tokenX.Substring(0, PAD_TOKEN));
                var token = tokenX.Substring(PAD_TOKEN);
                
                _memoryCache.Remove(Key(key));

                if (--calls >= 1)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(AppSettings.CacheExpirationGetSeconds) // Cache expiration
                    };
                    tokenX = calls.ToString().PadLeft(PAD_TOKEN, '0') + token;
                    _log.Debug("GetToken - added key=" + key + ", tokenX=" + tokenX);
                    _memoryCache.Set(Key(key), tokenX, cacheEntryOptions);
                    return token;
                }
                else if (calls == 0)
                {
                    _log.Debug("GetToken - last call key=" + key + ", tokenX=" + tokenX);
                    return token;
                }
                _log.Error("GetToken rejected, calls=" + calls + ", key = " + key);
            }
            _log.Error("GetToken is null, key=" + key);
            return null;
        }

        private string Key(string key)
        {
            return GC.CacheKeyTokenPrefix + key;
        }
    }
}
