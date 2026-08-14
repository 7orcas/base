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
    [Authorize]
    [PermissionAtt(GC.PerUser)]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : BaseController
    {
        private readonly UserServiceI _userService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ServiceProvider"></param>
        /// <param name="UserService"></param>
        public UserController(IServiceProvider serviceProvider,
            UserServiceI userService) : base(serviceProvider)
        {
            _userService = userService;
        }

        [CrudAtt(GC.CrudIgnore)]  //ToDo
        [AuditListAtt(GC.EntityTypeUser)]
        [HttpGet("list")]
        public async Task<IActionResult> Get()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var users = await _userService.GetUserList();
            var list = new List<UserDto>();

            foreach (var user in users)
                list.Add(await _userService.PopulateAsList(user));
            
            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = list
            };
            return Ok(r);
        }

        /// <summary>
        /// Get User by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CrudAtt(GC.CrudIgnore)] //ToDo
        [AuditListAtt(GC.EntityTypeUser)]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetUser(long id)
        {
            var user = await _userService.GetUser(id);
            if (user == null)
            {
                return NotFound();
            }

            var userDto = await _userService.Populate(user);
            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = userDto
            };
            return Ok(r);
        }

        /// <summary>
        /// Update Org
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudUpdate)]
        [AuditListAtt(GC.EntityTypeOrg)]
        [HttpPost("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UserDto dto)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var user = await _userService.UpdateUser(dto);
            var userDto = await _userService.Populate(user);
            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = userDto
            };
            return Ok(r);
        }

    }
}