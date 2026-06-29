using Backend.Base.Token.Ent;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
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
        /// Login process creates this JWT token, saves it to cache and returns a key to the token.
        /// The key is a one time use key to get the token from cache. The token is used to create a JWT token which is returned to the client.
        /// </summary>
        /// <param name="tv"></param>
        /// <returns></returns>
        public string CreateJWToken(TokenValues tv)
        {
            return CreateToken(tv, GC.TokenType.JWT);
        }

        public string CreateResetRequestToken(TokenValues tv)
        {
            return CreateToken(tv, GC.TokenType.ResetRequest);
        }

        private string CreateToken(TokenValues tv, GC.TokenType type)
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
                expires: DateTime.UtcNow.AddMinutes(AppSettings.AccessTokenMinutes),
                //expires: DateTime.UtcNow.AddSeconds(AppSettings.AccessTokenMinutes), TESTING
                signingCredentials: creds
                );

            var tt = type == GC.TokenType.JWT ? "JWT" : "ResetResquest";
            _log.Debug("CreateToken Type {Type} Username {Username} OrgNr {OrgNr} SessionKey {SessionKey}",
                tt, tv.Username, tv.OrgNr, tv.SessionKey);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            if (type == GC.TokenType.ResetRequest)
                return tokenString;

            var tokenKey = Guid.NewGuid().ToString();
            
            string tokenX = AppSettings.MaxGetTokenCalls.ToString().PadLeft(PAD_TOKEN, '0') + tokenString;
            _log.Debug("AddTokenToCache TokenKey {TokenKey} Token {Token}", tokenKey, tokenX);

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(AppSettings.CacheExpirationAddSeconds) // Cache expiration
            };
            _memoryCache.Set(TokenKey(tokenKey), tokenX, cacheEntryOptions);

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
        

        /// <summary>
        /// Login process calls this method to get the actual JWT token
        /// The number of calls is limited by appsetting value (default = 1)
        /// </summary>
        /// <param name="tokenKey"></param>
        /// <returns></returns>
        public string? GetJWToken(string tokenKey)
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

        /**
        * Refresh tokens.
        * 1. Create a new JWT token
        * 2. Create a new refresh token and save it to database
        * 2. Revoke the old refresh token
        */
        public async Task<(string jwToken, RefreshToken refreshToken)> RefreshToken(string refreshTokenString, string ipAddress, string revokedBy)
        {
            var refresh = await GetRefreshToken(refreshTokenString, revokedBy);

            if (refresh == null)
            {
                _log.Debug("RefreshToken-Failed Token {Token}", refreshTokenString);
                return (null, null);
            }

            var tv = new TokenValues
            {
                IpAddress = ipAddress,
                Username = refresh.Username,
                SessionKey = refresh.SessionKey,
                OrgNr = refresh.OrgNr,
            };

            var jwTokenKey = CreateJWToken(tv);
            var jwTokenNew = GetJWToken(jwTokenKey);
            var refreshTokenNew = await CreateRefreshToken(tv);
            
            _log.Debug("RefreshToken OldToken {OldToken} NewToken {NewToken}", refreshTokenString, refreshTokenNew.TokenString());

            return (jwTokenNew, refreshTokenNew);
        }


        public async Task<RefreshToken> CreateRefreshToken(TokenValues tv)
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

            token = await _tokenRepo.SaveRefreshToken(token);
            _log.Debug("RefreshToken-Create Token {Token}", token.TokenString());

            return token;
        }

        private async Task<RefreshToken?> GetRefreshToken(string tokenString, string revokedBy)
        {
            var result = GetRefreshTokenFromComposite(tokenString);
            var token = await _tokenRepo.LoadRefreshToken(result.Id);
            
            if (token != null
                && token.Token.ToString() == result.Guid.ToString() 
                && token.Expires > DateTime.UtcNow 
                && token.Revoked == null)
            {
                _tokenRepo.RevokeRefreshToken(result.Id, revokedBy);
                _log.Debug("GetRefreshToken-Found Token {Token}", tokenString);
                return token;
            }

            _log.Error("GetRefreshToken-IsNull Token {Token}", tokenString);
            return null;
        }


        private (Guid Guid, long Id) GetRefreshTokenFromComposite(string tokenString)
        {
            if (string.IsNullOrWhiteSpace(tokenString))
            {
                _log.Error("Invalid RefreshTokenString {tokenString}", tokenString);
                throw new ArgumentException("Input cannot be null or empty.");
            }


            int lastDash = tokenString.LastIndexOf('-');

            if (lastDash < 0)
                throw new FormatException("Invalid format.");

            string guidPart = tokenString.Substring(0, lastDash);
            string idPart = tokenString.Substring(lastDash + 1);

            if (!Guid.TryParse(guidPart, out var guid))
            {
                _log.Error("Invalid RefreshTokenString {tokenString}", tokenString);
                throw new FormatException("Invalid GUID.");
            }

            if (!long.TryParse(idPart, out var id))
            {
                _log.Error("Invalid RefreshTokenString {tokenString}", tokenString);
                throw new FormatException("Invalid ID.");
            }

            return (guid, id);
        }



    }
}
