using Common.DTO.Base;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Caching.Memory;
using MudBlazor;
using Newtonsoft.Json;
using System.Reflection.Emit;
using GC = FrontendServer.GlobalConstants;

/// <summary>
/// Scoped Menu service for the client
/// Created: Oct 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>
namespace FrontendServer.Base.Menu
{
    public class MenuService : BaseService
    {
        private readonly UrlService _urlService;
        private readonly IMemoryCache _cache;

        public MenuService(ProtectedSessionStorage session,
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache,
            UrlService urlService)
        {
            _session = session;
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _urlService = urlService;

            if (_cache.TryGetValue(GC.MainMenuCacheKey, out List<string> mainMenuItemsX))
                mainMenuItems = mainMenuItemsX;
        }

        private List<string> mainMenuItems;
                
        private bool IsInitialising = false;

        public bool IsInitialized => mainMenuItems != null;


        public bool IsMenu(string key) => mainMenuItems != null && key != null && mainMenuItems.Contains(key);

        public async Task ForceInitialise()
        {
            mainMenuItems = null;
            _cache.Remove(GC.MainMenuCacheKey);
            await Initialise();
        }

        public async Task Initialise()
        {
            try
            {
                var client = await GetClient();

                if (!IsInitialized && !IsInitialising)
                {
                    IsInitialising = true;
                    var url = _urlService.GetUrl("MainMenu");
                    var rl = await client.GetAsync(url);
                    var rls = await rl.Content.ReadAsStringAsync();
                    var rdto = JsonConvert.DeserializeObject<_ResponseDto>(rls);

                    if (rdto.Valid)
                    {
                        mainMenuItems = JsonConvert.DeserializeObject<List<string>>(rdto.Result.ToString());
                        _cache.Set(GC.MainMenuCacheKey, mainMenuItems);
                    }
                    IsInitialising = false;
                }
            }
            catch
            {
            }
        }

    }
}
