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
        [HttpPost("list")]
        public async Task<IActionResult> Get([FromBody] UserSearch search)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var users = await _userService.GetUserList(search);
            var list = new List<UserDto>();

            foreach (var user in users)
                list.Add(await _userService.PopulateList(user));
            
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

            var session = HttpContext.Items["session"] as SessionEnt;
            var userDto = await _userService.Populate(user);
            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = userDto
            };
            return Ok(r);
        }

        /// <summary>
        /// Update User
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudIgnore)] //ToDo
        [AuditListAtt(GC.EntityTypeUser)]
        [HttpPost("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateRequest<List<UserDto>> update)
        {
            var session = HttpContext.Items["session"] as SessionEnt;

            var list = update.Updates as List<UserDto>;
            var listU = new List<UserDto> ();


            //Validate //ToDo
            var vals = new List<ValidationDto> ();
            foreach (var dto in list)
            {
                var messages = await _userService.ValidateUser(session, dto);
                if (messages.Count > 0)
                    vals.Add(new ValidationDto { Id = dto.Id, Messages = messages });
            }

            if (vals.Count > 0)
            {
                var v = new _ResponseDto
                {
                    Valid = false,
                    Validations = vals
                };
                return Ok(v);
            }

            foreach (var dto in list)
            {
                var user = await _userService.UpdateUser(dto);
                if (user != null)
                    listU.Add(await _userService.Populate(user));
            }

            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = listU
            };
            return Ok(r);
        }

        [CrudAtt(GC.CrudIgnore)]  //ToDo
        [AuditListAtt(GC.EntityTypeUser)]
        [HttpGet("new")]
        public async Task<IActionResult> New()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var userDto = await _userService.NewUser(session);
            
            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = userDto
            };
            return Ok(r);
        }

    }
}