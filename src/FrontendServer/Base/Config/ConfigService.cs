using Common.DTO.Base;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using GC = FrontendServer.GlobalConstants;

/// <summary>
/// Scoped config service
/// Created: June 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>
namespace FrontendServer.Base.Config
{
    public class ConfigService : BaseService
    {
        public event Action? OnInitialized;

        public ConfigService(ProtectedSessionStorage session,
            IHttpClientFactory httpClientFactory,
            UrlService urlService)            
        {
            _session = session;
            _httpClientFactory = httpClientFactory;
            _urlService = urlService;
        }

        public async Task InitializeFromCache()
        {
            if (config == null)
            {
                var result = await _session.GetAsync<AppConfigDto> (GC.ConfigCacheKey);

                if (result.Success)
                    config = result.Value;
            }

            if (loginParameters == null)
            {
                var result = await _session.GetAsync <LoginParameters> (GC.LoginParamsCacheKey);

                if (result.Success)
                    loginParameters = result.Value;
            }

        }

        private AppConfigDto? config;
        public LoginParameters? loginParameters {  get; set; }
        public bool IsInitialized => config != null;
        public AppConfigDto? Config => config;
        public bool IsDebugMode => config != null && config.DebugMode;
        public bool IsDebugBorder => false;

        public void Set(AppConfigDto config) => this.config = config;

        public async Task<AppConfigDto> Initialise()
        {
            try
            {
                var client = await GetClient();
                var url = _urlService.GetUrl("ClientConfig");
                var cl = await client.GetAsync(url);
                var cls = await cl.Content.ReadAsStringAsync();
                var cdto = JsonConvert.DeserializeObject<_ResponseDto>(cls);
                var cjson = cdto.Result.ToString();
                config = JsonConvert.DeserializeObject<AppConfigDto>(cjson);

                await _session.SetAsync(GC.ConfigCacheKey, config);

                OnInitialized?.Invoke(); // Notify subscribers
                return config;
            }
            catch 
            {
                return null;
            }
        }

        public async Task<LoginParameters> SetLoginParameters(string? tokenKey, string? token, string? appUrls)
        {
            loginParameters = new LoginParameters();
            loginParameters.tokenkey = tokenKey;
            loginParameters.token = token;
            loginParameters.appUrls = appUrls;
            loginParameters.Set(config);

            await _session.SetAsync(GC.LoginParamsCacheKey, loginParameters);
            return loginParameters;
        }

        public async Task SetLoginParameters(LoginParameters loginParams)
        {
            loginParameters = loginParams;
            await _session.SetAsync(GC.LoginParamsCacheKey, loginParameters);
        }
    }
}
