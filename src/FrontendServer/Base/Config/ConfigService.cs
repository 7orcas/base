using Common.DTO.Base;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
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

        private AppConfigDto? config;
        public LoginParameters loginParameters {  get; private set; }

        public ConfigService(ProtectedSessionStorage session,
            IHttpClientFactory httpClientFactory)            
        {
            _session = session;
            _httpClientFactory = httpClientFactory;
        }

        public bool IsInitialized => config != null;
        public AppConfigDto? Config => config;
        public bool IsDebugMode => config != null && config.DebugMode;
        
        public void Set(AppConfigDto config) => this.config = config;

        public async Task<AppConfigDto> Initialise()
        {
            try
            {
                var client = await GetClient();
                var cl = await client.GetAsync(GC.URL_config);
                var cls = await cl.Content.ReadAsStringAsync();
                var cdto = JsonConvert.DeserializeObject<_ResponseDto>(cls);
                var cjson = cdto.Result.ToString();
                config = JsonConvert.DeserializeObject<AppConfigDto>(cjson);
config.DebugMode = true; //DELETE ME

                OnInitialized?.Invoke(); // Notify subscribers
                return config;
            }
            catch 
            {
                return null;
            }
        }

        public LoginParameters SetLoginParameters(string? tokenKey, string? token)
        {
            loginParameters = new LoginParameters();
            loginParameters.tokenkey = tokenKey;
            loginParameters.token = token;
            loginParameters.Set(config);
            return loginParameters;
        }

    }
}
