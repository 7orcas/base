using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Caching.Memory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FrontendServer.Base._Base
{
    public class StartupService : BaseService
    {
        private ConfigService _ConfS;
        private PermissionService _PermS;
        private LabelService _LS;
        private UrlService _UrlS;
        private MenuService _MS;

        public StartupService(ProtectedSessionStorage session,
            IHttpClientFactory httpClientFactory,
            ConfigService ConfS,
            PermissionService PermS,
            LabelService LS,
            UrlService UrlS,
            MenuService MS)
        {
            _session = session;
            _httpClientFactory = httpClientFactory;
            _UrlS = UrlS;
            _ConfS = ConfS;
            _PermS = PermS;
            _LS = LS;
            _MS = MS;
        }

        public async Task InitialiseServicesFromCache()
        {
            await _UrlS.InitializeFromCache();
            await _ConfS.InitializeFromCache();
            await _PermS.InitializeFromCache();
            //await _LS.InitializeFromCache();
            await _MS.InitializeFromCache();
        }


    }
}
