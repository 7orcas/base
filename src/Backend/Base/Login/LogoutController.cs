using Backend.App.Machines;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GC = Backend.GlobalConstants;

namespace Backend.Base.Login
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LogoutController :  BaseController
    {
        private readonly LoginServiceI _loginService;
        private readonly SessionServiceI _sessionService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ServiceProvider"></param>
        /// <param name="MachineService"></param>
        public LogoutController(IServiceProvider serviceProvider,
            LoginServiceI loginService,
            SessionServiceI sessionService) : base(serviceProvider)
        {
            _loginService = loginService;
            _sessionService = sessionService;
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var ses = HttpContext.Items["session"] as SessionEnt;
            _auditService.LogInOut(ses.SourceApp, ses.Org.Nr, ses.UserAccount.Id, GC.EntityTypeLogout);
            _sessionService.RemoveSession(ses.Key);
            return Ok("ok");
        }

    }
}