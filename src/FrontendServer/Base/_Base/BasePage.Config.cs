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
            if (_configDto != null) return;
            _configDto = ConfS.Config;
        }

        public async Task<ConfigDto> GetConfig() => _configDto;
        
        public void SetConfig(ConfigDto config)
        {
            _configDto = config;
            ConfS.Set(config);
        }

        public string GetDebugBorder(string color)
        {
            if (ConfS == null) return string.Empty;
            return ConfS.IsDebugBorder ? " border:5px solid " + color + ";": string.Empty;
        }

    }
}
