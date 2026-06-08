using Backend.Base.Token.Ent;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using GC = Backend.GlobalConstants;

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
        private readonly TokenRepoI _tokenRepo;
        private readonly IMemoryCache _memoryCache;
        private const int PAD_TOKEN = 5;

        public TokenService(IServiceProvider serviceProvider,
            IMemoryCache memoryCache,
            TokenRepoI tokenRepo)
            : base(serviceProvider)
        {
            _memoryCache = memoryCache;
            _tokenRepo = tokenRepo;
        }


        /// <summary>
        /// Login process creates this token, saves it to cache and returns a key to the token.
        /// The key is a one time use key to get the token from cache. The token is used to create a JWT token which is returned to the client.
        /// </summary>
        /// <param name="tv"></param>
        /// <returns></returns>
        public string CreateToken(TokenValues tv)
        {
            var claims = new[]
            {
                new Claim("Key", tv.SessionKey),
                new Claim("Org", "" + tv.OrgNr),
                new Claim("Ip", "" + tv.IpAddress),
                new Claim("User", "" + tv.Username),
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenParameters._Key));
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: TokenParameters._Issuer,
                audience: TokenParameters._Audience,
                claims: claims,
                //expires: DateTime.UtcNow.AddMinutes(AppSettings.AccessTokenMinutes),
                expires: DateTime.UtcNow.AddSeconds(AppSettings.AccessTokenMinutes),
                signingCredentials: creds
                );

            _log.Debug("CreateToken TokenValues {TokenValues}", tv.ToLogString());

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var tokenKey = Guid.NewGuid().ToString();
            CacheTokenKey(tokenKey, tokenString);

            return tokenKey;
        }

        /// <summary>
        /// Every call will use this method via the Interceptor
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public TokenValues DecodeToken(string token)
        {
            token = token.Trim();
            var tokenHandler = new JwtSecurityTokenHandler();

            TokenValues tv = new TokenValues();
            try
            {
                // Validate token and decode
                var principal = tokenHandler.ValidateToken(token, TokenParameters.GetParameters(), out var validatedToken);

                // Extract claims
                tv.SessionKey = principal.FindFirst("Key")?.Value.ToString();
                int.TryParse(principal.FindFirst("Org")?.Value, out int orgNr);
                tv.OrgNr = orgNr;
                tv.IpAddress = principal.FindFirst("Ip")?.Value.ToString();
                tv.Username = principal.FindFirst("User")?.Value.ToString();

                _log.Debug("DecodeToken SessionKey {SessionKey} Token {Token}", tv.SessionKey, token);
                return tv;
            }
            catch (Exception ex)
            {
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                _log.Debug("Token expires at {Expiry}", jwt.ValidTo);
                _log.Error("TokenValidationFailed Exception {Exception}", ex.Message);
                return null;
            }
        }

        private void CacheTokenKey(string key, string token)
        {
            string tokenX = AppSettings.MaxGetTokenCalls.ToString().PadLeft(PAD_TOKEN, '0') + token;
            _log.Debug("AddTokenToCache TokenKey {TokenKey} Token {Token}", key, tokenX);

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(AppSettings.CacheExpirationAddSeconds) // Cache expiration
            };
            _memoryCache.Set(TokenKey(key), tokenX, cacheEntryOptions);
        }

        /// <summary>
        /// Login process calls this method to get the actual JWT token
        /// The number of calls is limited by appsetting value (default = 1)
        /// </summary>
        /// <param name="tokenKey"></param>
        /// <returns></returns>
        public string? GetToken(string tokenKey)
        {
            _log.Debug("GetToken TokenKey {TokenKey}", tokenKey);

            if (_memoryCache.TryGetValue(TokenKey(tokenKey), out var cachedValue))
            {
                var tokenX = cachedValue.ToString();
                var calls = int.Parse(tokenX.Substring(0, PAD_TOKEN));
                var token = tokenX.Substring(PAD_TOKEN);
                
                _memoryCache.Remove(TokenKey(tokenKey));

                //This logic is not used, max calls is = 1 (number calls is controlled by appsettings) and can be removed DELETE ME
                if (--calls >= 1)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(AppSettings.CacheExpirationGetSeconds) // Cache expiration
                    };
                    tokenX = calls.ToString().PadLeft(PAD_TOKEN, '0') + token;
                    _log.Debug("GetToken-AddKey TokenKey {TokenKey} TokenX {TokenX}", tokenKey, tokenX);
                    _memoryCache.Set(TokenKey(tokenKey), tokenX, cacheEntryOptions);
                    return token;
                }
                else if (calls == 0)
                {
                    _log.Debug("GetToken-LastCall TokenKey {TokenKey} TokenX {TokenX}", tokenKey, tokenX);
                    return token;
                }
                _log.Error("GetToken-Rejected TokenKey {TokenKey} NumberCalls {NumberCalls}", tokenKey, calls);
            }

            _log.Error("GetToken-IsNull TokenKey {TokenKey}", tokenKey);
            return null;
        }

        private string TokenKey(string key)
        {
            return GC.CacheKeyTokenPrefix + key;
        }


private string RefreshKey(string key)
{
    return GC.CacheKeyRefreshPrefix + key;
}

        public RefreshToken CreateRefreshToken(TokenValues tv)
        {
            var token = new RefreshToken
            {
                Token = Guid.NewGuid(),
                Expires = DateTime.UtcNow.AddDays(AppSettings.RefreshTokenDays),
                Created = DateTime.UtcNow,
                CreatedByIp = tv.IpAddress,
                Username = tv.Username,
                SessionKey = tv.SessionKey,
                OrgNr = tv.OrgNr
            };

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(AppSettings.RefreshTokenDays)
            };
            _memoryCache.Set(RefreshKey(token.Token.ToString()), token, cacheEntryOptions);

            _tokenRepo.SaveRefreshToken(token);

            return token;
        }

        public async Task<RefreshToken?> GetRefreshToken(string tokenKey)
        {
            _log.Debug("GetRefreshToken TokenKey {TokenKey}", tokenKey);

            var tokenx = await _tokenRepo.LoadRefreshToken(tokenKey);
            if (tokenx != null)
                _tokenRepo.RevokeRefreshToken(tokenx);
            
            
            if (_memoryCache.TryGetValue(RefreshKey(tokenKey), out var cachedValue))
            {
                var token = (RefreshToken)cachedValue;
                
                _memoryCache.Remove(TokenKey(tokenKey));
                _log.Debug("GetToken-LastCall TokenKey {TokenKey} TokenX {TokenX}", tokenKey, token.Token);
                return token;
            }

            _log.Error("GetRefreshToken-IsNull TokenKey {TokenKey}", tokenKey);
            return null;
        }


    }
}
