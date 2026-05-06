
using FrontendServer.Base.Label;

using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

/// <summary>
/// Scoped http service for the client
/// Convienence class to get an http client
/// Created: July 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>
namespace FrontendServer.Base.Http
{
    public class HttpService : BaseService
    {
        public HttpService(ProtectedSessionStorage session,
                   IHttpClientFactory httpClientFactory,
                   ConfigService configService)
        {
            _session = session;
            _httpClientFactory = httpClientFactory;
            _configService = configService;
        }
    }
}
