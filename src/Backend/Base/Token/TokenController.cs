using Backend.App.Machines;
using Common.DTO.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

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
            token = LoginTokenDto.TOKEN_PREFIX + token;

            _log.Debug("Get token controller, Token=" + token);
            return Ok(token);
        }
    }

}
