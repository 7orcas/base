using Common.DTO.Base;
using GC = FrontendServer.GlobalConstants;

/// <summary>
/// Configuration methods for BasePage
/// Created: June 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace FrontendServer.Base._Base
{
    //ToDo Is this class required?
    public partial class BasePage
    {
        protected async Task SetConfig()
        {
            if (_config != null) return;
            _config = ConfS.Config;
        }

        public async Task<AppConfigDto> GetConfig() => _config;
        
        public void SetConfig(AppConfigDto config)
        {
            _config = config;
            ConfS.Set(config);
        }


    }
}
