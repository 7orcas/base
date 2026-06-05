using Backend.App.Machines;
using Common.DTO.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.IdentityModel.Tokens.Jwt;

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
        public IActionResult GetToken([FromQuery] string key)
        {
            var token = _tokenService.GetToken(key);

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("No token found in session");
            }
            token = LoginSuccessDto.TOKEN_PREFIX + token;

            var refreshToken = _tokenService.CreateRefreshToken();

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
                    RefreshToken = refreshToken,
                    AccessTokenExpiry = expiry
                }
            };
            return Ok(r);
        }
    }

}
