using Common.DTO.Base;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
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
    //[Authorize]
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

        [CrudAtt(GC.AuditIgnore)]
        [AuditIgnoreAtt]
        [HttpGet("search")]
        public async Task<IActionResult> GetSearch()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var s = await GetUserCache<UserSearch>(session, "UserSearch");
            s = s ?? new UserSearch();
            return Ok(new _ResponseDto(s));
        }


        /// <summary>
        /// Get User list
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [CrudAtt(GC.CrudReadList)]
        [AuditListAtt(GC.CrudReadList)]
        [UserCacheAtt("UserSearch")]
        [HttpPost("list")]
        public async Task<IActionResult> GetList([FromBody] UserSearch search)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var users = await _userService.GetUserList(session, search);
            var list = new List<UserDto>();

            foreach (var user in users)
                list.Add(await _userService.PopulateList(session, user));

            return Ok(new _ResponseDto(list));
        }

        /// <summary>
        /// Get User by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CrudAtt(GC.CrudRead)] 
        [AuditListAtt(GC.CrudRead)]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            var session = HttpContext.Items["session"] as SessionEnt;
            var userDto = await _userService.Populate(session, user);
            return Ok(new _ResponseDto(userDto));
        }

        /// <summary>
        /// Update, create and delete a User
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        [CrudAtt(GC.CrudUpdate)] 
        [AuditListAtt(GC.CrudUpdate)]
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] UpdateRequest<List<UserDto>> update)
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var list = update.Updates as List<UserDto>;

            //Validate 
            var vals = await _userService.ValidateUser(session, list);
            if (vals.Count > 0)
                return Ok(new _ResponseDto
                {
                    Valid = false,
                    Validations = vals
                });
            

            //Do updates
            var listBefore = new List<UserDto>();
            var listUpdated = new List<UserDto> ();
            foreach (var dto in list)
            {
                var user = await _userService.GetUserForUpdate(dto);
                if (user == null) continue;

                var before = await _userService.Populate(session, user);
                before.IsDelete = dto.IsDelete;
                listBefore.Add(before);

                user = await _userService.UpdateUser(user, dto);
                if (user != null)
                {
                    before.Id = user.Id; //link them
                    listUpdated.Add(await _userService.Populate(session, user));
                }
            }

            return Ok(new _ResponseDto(listBefore, listUpdated));
        }

        /// <summary>
        /// Get a new User entity with default values
        /// Note it is not saved to the database yet, it is just a template for creating a new user
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudCreate)] 
        [AuditListAtt(GC.CrudCreate)]
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