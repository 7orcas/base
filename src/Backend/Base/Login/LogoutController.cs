using Microsoft.AspNetCore.Mvc;

namespace Backend.Base.Login
{
    /// <summary>
    /// Logout a client
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LogoutController :  BaseController
    {
        private readonly LoginServiceI _loginService;
        private readonly SessionServiceI _sessionService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="loginService"></param>
        /// <param name="sessionService"></param>
        public LogoutController(IServiceProvider serviceProvider,
            LoginServiceI loginService,
            SessionServiceI sessionService) : base(serviceProvider)
        {
            _loginService = loginService;
            _sessionService = sessionService;
        }

        /// <summary>
        /// Logout client
        /// </summary>
        /// <returns></returns>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var ses = HttpContext.Items["session"] as SessionEnt;
            if (ses != null)
            {
                _auditService.LogOut(ses);
                await _sessionService.RemoveSession(ses.Key);
            }
            return Ok("ok");
        }

    }
}