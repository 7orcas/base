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
    [PermissionAtt(GC.PerUser)] //Hard coded permission - user must be flagged as User Admin
    [ApiController]
    [AuditListAtt(GC.EntityTypeUser)]
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

        /// <summary>
        /// Get User entity field definitions
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudRead)]
        [AuditIgnoreAtt]
        [HttpGet("definition")]
        public async Task<IActionResult> GetDefinition()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var def = await _userService.GetDefinition(session);
            
            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = def
            };
            return Ok(r);
        }

        /// <summary>
        /// Get User list
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [CrudAtt(GC.CrudReadList)]
        [AuditListAtt(GC.EntityTypeUser, GC.CrudReadList)]
        [HttpPost("list")]
        public async Task<IActionResult> GetList([FromBody] UserSearch search)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var users = await _userService.GetUserList(search);
            var list = new List<UserDto>();

            foreach (var user in users)
                list.Add(await _userService.PopulateList(session, user));
            
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
        [CrudAtt(GC.CrudRead)] 
        [AuditListAtt(GC.EntityTypeUser, GC.CrudRead)]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetUserById(long id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            var session = HttpContext.Items["session"] as SessionEnt;
            var userDto = await _userService.Populate(session, user);
            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = userDto
            };
            return Ok(r);
        }

        /// <summary>
        /// Update, create and delete a User
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        [CrudAtt(GC.CrudUpdate)] 
        [AuditListAtt(GC.EntityTypeUser, GC.CrudUpdate)]
        [HttpPost("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateRequest<List<UserDto>> update)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var list = update.Updates as List<UserDto>;

            //Validate 
            var vals = await _userService.ValidateUser(session, list);
            if (vals.Count > 0)
            {
                var v = new _ResponseDto
                {
                    Valid = false,
                    Validations = vals
                };
                return Ok(v);
            }

            //Do updates
            var listU = new List<UserDto> ();
            foreach (var dto in list)
            {
                var user = await _userService.UpdateUser(dto);
                if (user != null)
                    listU.Add(await _userService.Populate(session, user));
            }

            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = listU
            };
            return Ok(r);
        }

        /// <summary>
        /// Get a new User entity with default values
        /// Note it is not saved to the database yet, it is just a template for creating a new user
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudCreate)] 
        [AuditListAtt(GC.EntityTypeUser, GC.CrudCreate)]
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