using Backend.Base.Token.Ent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("token")]
        public async Task<IActionResult> GetToken([FromQuery] string key)
        {
            var ipAddress = GetClientIp();

            var token = _tokenService.GetJWToken(key);

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("No token found in session");
            }

            var tv = _tokenService.DecodeJWToken(token);
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

        [HttpGet("refresh/{refreshTokenString}")]
        public async Task<IActionResult> RefreshToken(string refreshTokenString)
        {
            var ipAddress = GetClientIp();
            var refresh = await _tokenService.GetRefreshToken(refreshTokenString);

            var tv = new TokenValues {
                IpAddress = ipAddress,
                Username = refresh.Username,
                SessionKey = refresh.SessionKey,
                OrgNr = refresh.OrgNr,
            };

            var jwTokenKey = _tokenService.CreateJWToken(tv);
            var jwTokenNew = _tokenService.GetJWToken(jwTokenKey);
            var refreshTokenNew = await _tokenService.CreateRefreshToken(tv);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(jwTokenNew);
            var expiry = jwt.ValidTo;

            _log.Debug("Get token controller, Token {Token} expiry {expiry}", jwTokenNew, expiry);

            var r = new _ResponseDto
            {
                SuccessMessage = "Got Token Ok",
                Result = new LoginTokenDto
                {
                    AccessToken = jwTokenNew,
                    RefreshToken = refreshTokenNew.TokenString(),
                    AccessTokenExpiry = expiry
                }
            };
            return Ok(r);
        }

        [Authorize]
        [CrudAtt(GC.CrudIgnore)]
        [HttpGet("test")]
        public IActionResult TestToken()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = "Ok"
            };
            return Ok(r);
        }

    }

}
