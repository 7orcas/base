using Microsoft.AspNetCore.Mvc;
using GC = Backend.GlobalConstants;

/// <summary>
/// Url controller for any client
/// Created: Oct 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.App.Base.Menu
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : BaseController
    {

        private readonly MenuServiceI _menuService;

        /// <summary>
        /// Constructor
        /// </summary>
        public MenuController(IServiceProvider serviceProvider,
            MenuServiceI menuService) 
            : base(serviceProvider)    
        {
            _menuService = menuService;
        }

        
        [HttpGet("mainmenu")]
        public async Task<IActionResult> GetMainMenu()
        {
            var session = HttpContext.Items["session"] as SessionEnt;

            var list = _menuService.GetMenuItemIds(session);

            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = list
            };
            return Ok(r);
        }


    }

}