using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using GC = FrontendServer.GlobalConstants;

namespace FrontendServer.Base.Logout
{

    public class LogoutService : BaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly NavigationManager _navigation;

        public LogoutService(ProtectedSessionStorage session,
            NavigationManager navigationManager,
            IHttpClientFactory httpClientFactory)
        {
            _session = session;
            _navigation = navigationManager;
            _httpClientFactory = httpClientFactory;
        }


        public async Task LogoutAsync()
        {
            var client = _httpClientFactory.CreateClient(GC.AuthorizedClientKey);
            var token = await _session.GetAsync<string>(GC.TokenCacheKey);


            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(GC.BearerKey, token.Value);

            await _session.DeleteAsync(GC.TokenCacheKey);
            var response = await client.PostAsync(GC.URL_logout, null);

            _navigation.NavigateTo("https://localhost:7289", forceLoad: true);
        }

    }
}
