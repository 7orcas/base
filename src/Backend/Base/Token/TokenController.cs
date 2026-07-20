using Backend.Base.Token.Ent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Superpower.Model;
using Superpower.Parsers;
using System.IdentityModel.Tokens.Jwt;
using GC = Backend.GlobalConstants;

namespace Backend.Base.Token
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : BaseController
    {
        private readonly TokenServiceI _tokenService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="TokenService"></param>
        public TokenController(IServiceProvider serviceProvider, 
            TokenServiceI tokenService) : base(serviceProvider)
        {
            _tokenService = tokenService;
        }

        /// <summary>
        /// Get refresh token from the login key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("token")]
        public async Task<IActionResult> GetToken([FromQuery] string key)
        {
            var ipAddress = GetClientIp();

            var token = _tokenService.GetJWToken(key);

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("No token found in session");
            }

            var tv = _tokenService.DecodeToken(token);

            if (tv.IpAddress != ipAddress)
            {
                _log.Error("IP adress mismatch, Token {Token} ipAddress {ipAddress}", token, ipAddress);
                return Unauthorized("IP address mismatch");
            }

            var refreshToken = await _tokenService.CreateRefreshToken(tv);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var expiry = jwt.ValidTo;

            _log.Debug("Get token controller, Token {Token} expiry {expiry}", token, expiry);

            var r = new _ResponseDto
            {
                SuccessMessage = "Got Token Ok",
                Result = new LoginTokenDto
                {
                    AccessToken = token,
                    RefreshToken = refreshToken.TokenString(),
                    AccessTokenExpiry = expiry
                }
            };
            return Ok(r);
        }

        [AllowAnonymous]
        [HttpGet("refreshcurrent/{refreshTokenString}")]
        public async Task<IActionResult> RefreshCurrentToken(string refreshTokenString)
        {
            var dto = await RefreshToken(refreshTokenString, "Current");
            return Ok(dto);
        }

        [AllowAnonymous]
        [HttpGet("refreshexpired/{refreshTokenString}")]
        public async Task<IActionResult> RefreshExpiredToken(string refreshTokenString)
        {
            var dto = await RefreshToken(refreshTokenString, "Expired");
            return Ok(dto);
        }

        private async Task<_ResponseDto> RefreshToken(string refreshTokenString, string revokedBy)
        {
            var ipAddress = GetClientIp();
            var result = await _tokenService.RefreshToken(refreshTokenString, ipAddress, revokedBy);

            if (result.jwToken == null || result.refreshToken == null)
            {
                return new _ResponseDto
                    {
                        Valid = false,
                        ErrorMessage = "No token - may have expired",  
                        StatusCode = GC.StatusCodeNotAuthorised 
                    };
            }

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(result.jwToken);
            var expiry = jwt.ValidTo;

            _log.Debug("Get token controller, Token {Token} expiry {expiry}", result.jwToken, expiry);

            var r = new _ResponseDto
            {
                SuccessMessage = "Got Token Ok",
                Result = new LoginTokenDto
                {
                    AccessToken = result.jwToken,
                    RefreshToken = result.refreshToken.TokenString(),
                    AccessTokenExpiry = expiry
                }
            };
            return r;
        }

        [Authorize]
        [CrudAtt(GC.CrudIgnore)]
        [HttpGet("test")]
        public IActionResult TestToken()
        {
            var authorizationHeader = HttpContext.Request.Headers["Authorization"].ToString();
            var expiry = DateTime.MinValue;
            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer"))
            {
                var jwToken = authorizationHeader.Substring("Bearer".Length);
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(jwToken.Trim());
                expiry = jwt.ValidTo;
            }

            double minutes = (expiry - DateTime.UtcNow).TotalMinutes;
            var refresh = minutes < 1.5;
            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = refresh
            };
            return Ok(r);
        }

    }

}
