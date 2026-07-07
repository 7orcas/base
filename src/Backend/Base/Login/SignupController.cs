using Backend.Base.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Serilog.Events;
using GC = Backend.GlobalConstants;

/// <summary>
/// Self Registration controller for the blazor client
/// Created: July 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Login
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignupController : BaseController
    {
        private readonly LoginServiceI _loginService;
        private readonly SignupServiceI _signupService;
        private readonly OrgServiceI _orgService;
        
        /// <summary>
        /// Constructor
        /// </summary>
        public SignupController(
            IServiceProvider serviceProvider,
            LoginServiceI loginService,
            SignupServiceI signupService,
            OrgServiceI orgService) : base(serviceProvider)
        {
            _loginService = loginService;
            _signupService = signupService;
            _orgService = orgService;
        }


        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            var ipAddress = GetClientIp();
            var res = await _signupService.SignupUser(ipAddress, request.UserName, request.Email, request.Password, request.OrgNr, request.LangCode);

            var r = new _ResponseDto
            {
                SuccessMessage = "Signup process",
                Valid = res.success,
                Result = res.message
            };
            return Ok(r);
        }

        [HttpGet("signupverifyemail")]
        public async Task<IActionResult> VerifyEmail([FromQuery] int orgNr, [FromQuery] string langCode, [FromQuery] int langVariant, [FromQuery] string email)
        {
            var ipAddress = GetClientIp();

            var rtn = await _signupService.VerifyEmail(ipAddress, email, orgNr, langCode);

            var r = new _ResponseDto
            {
                SuccessMessage = "Verify Email",
                Result = rtn.message,
                Valid = rtn.valid,
            };
            return Content(GetEmailVerifiedHtml(), "text/html");
        }

        public static string GetEmailVerifiedHtml()
        {
            return """
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Email Verified</title>
    <style>
        body {
            margin: 0;
            font-family: Arial, sans-serif;
            background: #f4f6f8;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
        }

        .container {
            text-align: center;
            background: white;
            padding: 40px;
            border-radius: 12px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.1);
        }

        .icon {
            font-size: 60px;
            color: #28a745;
            margin-bottom: 20px;
        }

        h1 {
            margin: 0 0 10px;
            color: #333;
        }

        p {
            margin: 0;
            color: #666;
            font-size: 18px;
        }
    </style>
</head>
<body>
    <div class=""container"">
        
        <h1>Email Verified</h1>
        <p>You are now able to log in.</p>
    </div>
</body>
</html>
""";
        }



    }

}