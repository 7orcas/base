
// DTO field definitions

using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Newtonsoft.Json;
using GC = FrontendServer.GlobalConstants;

namespace FrontendServer.Base.Config
{
    public class FieldDefinitionService : BaseService
    {
        public event Action? OnInitialized;
                
        public FieldDefinitionService(ProtectedSessionStorage session,
            IHttpClientFactory httpClientFactory)
        {
            _session = session;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<DefinitionDto> Initialise(string url)
        {
            var f = await GetFieldFromStorage(url);
            if (f != null) return f;

            try
            {
                var client = await GetClient();
                var cl = await client.GetAsync(url);
                var cls = await cl.Content.ReadAsStringAsync();
                var cdto = JsonConvert.DeserializeObject<_ResponseDto>(cls);
                var cjson = cdto.Result.ToString();

                await _session.SetAsync(GC.FieldConfigCacheKey + url, cjson);

                var fields = JsonConvert.DeserializeObject<DefinitionDto>(cjson);
                return fields;
            }
            catch
            {
                return null;
            }
        }

        public async Task<DefinitionDto?> GetFieldFromStorage(string url)
        {
            try
            {
                var store = await _session.GetAsync<string>(GC.FieldConfigCacheKey + url);
                if (!store.Success) return null;

                var fields = JsonConvert.DeserializeObject<DefinitionDto>(store.Value);
                return fields;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
