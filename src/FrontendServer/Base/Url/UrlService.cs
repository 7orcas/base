using Common.DTO.Base;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Caching.Memory;
using MudBlazor;
using Newtonsoft.Json;
using System.Reflection.Emit;
using GC = FrontendServer.GlobalConstants;

/// <summary>
/// Scoped URL service for the client
/// Created: Oct 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>
namespace FrontendServer.Base.Config
{
    public class UrlService : BaseService
    {
        public UrlService(ProtectedSessionStorage session,
            IHttpClientFactory httpClientFactory)
        {
            _session = session;
            _httpClientFactory = httpClientFactory;
        }

        public async Task InitializeFromCache()
        {
            if (urls != null) return;

            var result = await _session.GetAsync<Dictionary<string, string>>(GC.UrlCacheKey);

            if (result.Success)
                urls = result.Value;
        }


        private Dictionary<string, string> urls;
                
        private bool IsInitialising = false;

        public bool IsInitialized => urls != null;


        public bool IsUrl(string urlKey) => urls != null && urlKey != null && urls.ContainsKey(urlKey);

        public string GetUrl(string urlKey) => IsUrl(urlKey) ? urls[urlKey] : urlKey;

        public string GetUrlSlash(string urlKey) => GetUrl(urlKey) + "/";
               

        public async Task Initialise(string appUrls)
        {
            try
            {
                var client = await GetClient();

                if (!IsInitialized && !IsInitialising)
                {
                    IsInitialising = true;
                    var rl = await client.GetAsync(appUrls);
                    var rls = await rl.Content.ReadAsStringAsync();
                    var rdto = JsonConvert.DeserializeObject<_ResponseDto>(rls);

                    if (rdto.Valid)
                    {
                        var urlsX = JsonConvert.DeserializeObject<List<UrlDto>>(rdto.Result.ToString());
                        var dic = new Dictionary<string, string>();
                        foreach (var l in urlsX)
                            dic.Add(l.Key, l.Url);

                        urls = dic;
                        await _session.SetAsync(GC.UrlCacheKey, urls);
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
