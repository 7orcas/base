using Microsoft.AspNetCore.Mvc;
using GC = Backend.GlobalConstants;

/// <summary>
/// Configuration is a combination of the org config and the user settings.
/// It can be used by the client to configuration options and functionality.
/// Created: May 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Config
{
    //[Authorize]
    [PermissionAtt(GC.PerConfig)]
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigController : BaseController
    {
        private readonly ConfigServiceI _ConfigService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ServiceProvider"></param>
        /// <param name="ConfigService"></param>
        public ConfigController(IServiceProvider serviceProvider,
            ConfigServiceI ConfigService) : base(serviceProvider)
        {
            _ConfigService = ConfigService;
        }

        /// <summary>
        /// Get client config
        /// This is a mix of organisation and user account config
        /// </summary>
        /// <returns></returns>
        [CrudAtt(GC.CrudIgnore)]
        [HttpGet("clientConfig")]
        public async Task<IActionResult> GetClientConfig()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var org = session.Org;

            var langCodeCurrent = session.LangCodeCurrent;
           // var isAdmin = session.IsAdmin;  << need to save this

            var langs = new List<LanguageConfigDto>();
            foreach (var lang in session.Languages)
            {
                langs.Add(new LanguageConfigDto {
                    LangCode = lang.LangCode,
                    IsUpdateable = lang.IsEditable,
                });
            }
            
            var r = new _ResponseDto
            {
                SuccessMessage = "Config Ok",
                Result = new AppConfigDto
                {
                    UniqueUserId = session.UserAccountId + 987123564,
                    UniqueSessionId = UniqueSessionId.GetId(),
                    
                    Org = new OrgConfigDto
                    {
                        Nr = session.Org.Nr,
                        Code = session.Org.Code,
                        Description = session.Org.Description,
                        Icon = session.Org.Icon
                    },

                    User = new UserConfigDto 
                    {
                        Username = session.Username,
                        IsService = session.IsService,
                       // IsAdmin = session.IsAdmin,
                    },

                    Languages = langs.ToArray(),
                    Label = new LabelConfigDto
                    {
                        LangCode = langCodeCurrent,
                        Variant = org.LangLabelVariant,
                        ShowTooltip = true,
                       // HighlightNoKey = isAdmin, //ToDo turn on/off
                        //IsAdmin = isAdmin, //ToDo turn on/off
                    },
                }
            };
            return Ok(r);
        }
    }
}