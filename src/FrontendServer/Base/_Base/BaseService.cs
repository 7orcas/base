using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using GC = FrontendServer.GlobalConstants;


/// <summary>
/// Base service utilities
/// Created: June 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace FrontendServer.Base._Base
{
    public abstract class BaseService
    {
        protected ProtectedSessionStorage _session;
        protected ConfigService _configService;
        protected IHttpClientFactory _httpClientFactory;

        public BaseService()
        {
        }

        public async Task<string?> GetToken()
        {
            try
            {
                var store = await _session.GetAsync<string>(GC.TokenCacheKey);
                return store.Value;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<HttpClient?> GetClient()
        {
            try
            {
                var token = await GetToken();
                var client = _httpClientFactory.CreateClient(GC.AuthorizedClientKey);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(GC.BearerKey, token);
                return client;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
