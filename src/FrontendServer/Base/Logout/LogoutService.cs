using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using GC = FrontendServer.GlobalConstants;

namespace FrontendServer.Base.Logout
{

    public class LogoutService : BaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly NavigationManager _navigation;
        private readonly ConfigService _configService;

        public LogoutService(ProtectedSessionStorage session,
            ConfigService configService,
            NavigationManager navigationManager,
            IHttpClientFactory httpClientFactory)
        {
            _session = session;
            _navigation = navigationManager;
            _configService = configService;
            _httpClientFactory = httpClientFactory;
        }


        public async Task LogoutAsync()
        {
            var client = _httpClientFactory.CreateClient(GC.AuthorizedClientKey);
            var token = await _session.GetAsync<string>(GC.TokenCacheKey);

        //To Do    HttpContextAccessor.HttpContext.Response.Cookies.Delete("remember_me");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(GC.BearerKey, token.Value);

            await _session.DeleteAsync(GC.TokenCacheKey);
            var response = await client.PostAsync(GC.URL_logout, null);
            var config = _configService.Config;

            _navigation.NavigateTo(config.UrlLogin, forceLoad: true);
        }

    }
}
