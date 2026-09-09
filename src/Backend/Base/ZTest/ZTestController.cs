using Common.DTO.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GC = Backend.GlobalConstants;

/// <summary>
/// User Admin controller
/// Created: August 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.User
{
    [PermissionAtt(GC.PerIgnore)] //Hard coded permission - user must be flagged as User Admin
    [ApiController]
    [Route("api/[controller]")]
    public class ZTestController : BaseController
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ServiceProvider"></param>
        /// <param name="UserService"></param>
        public ZTestController(IServiceProvider serviceProvider) 
            : base(serviceProvider) { }


        [AllowAnonymous]
        [CrudAtt(GC.CrudIgnore)]
        [HttpGet("test1")]
        public async Task<IActionResult> GetTest1()
        {
            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = "Test result: Allow anonymous"
            };
            return Ok(r);
        }

        [CrudAtt(GC.CrudIgnore)]
        [HttpGet("test2")]
        public async Task<IActionResult> GetTest2()
        {
            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = "Test result: Dont allow anonymous"
            };
            return Ok(r);
        }


    }
}