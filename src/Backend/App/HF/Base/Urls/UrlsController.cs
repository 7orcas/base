using Microsoft.AspNetCore.Mvc;
using GC = Backend.GlobalConstants;

/// <summary>
/// Url controller for any client
/// Created: Oct 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.App.HF.Base.Urls
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class UrlsController : BaseController
    {

        private readonly UrlsServiceI _urlsService;

        /// <summary>
        /// Constructor
        /// </summary>
        public UrlsController(IServiceProvider serviceProvider,
            UrlsServiceI urlsService) 
            : base(serviceProvider)    
        {
            _urlsService = urlsService;
        }

        
        [HttpGet("hf-list")]
        public async Task<IActionResult> GetList()
        {
            var session = HttpContext.Items["session"] as SessionEnt;

            var list = _urlsService.GetList(session);

            var r = new _ResponseDto
            {
                SuccessMessage = "Ok",
                Result = list
            };
            return Ok(r);
        }


    }

}