using Newtonsoft.Json;
using GC = FrontendLogin.GlobalConstants;

/// <summary>
/// Scoped label service for the client
/// Created: June 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>
namespace FrontendLogin.Label
{
    public class LabelService 
    {
        protected IHttpClientFactory _httpClientFactory;
        
        public LabelService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Dictionary<string, LangLabelDto>> InitialiseLabels(string langCode, int? variant)
        {
            var dic = new Dictionary<string, LangLabelDto>();
            try
            {
                var url = GC.URL_login_label
                                + "?langcode=" + langCode
                                + (variant.HasValue ? "&langvar=" + variant : "");

                var client = _httpClientFactory.CreateClient(GC.HTTP_Client);
                var response = await client.GetAsync(url);
                var r = await response.Content.ReadAsStringAsync();
                var dto = JsonConvert.DeserializeObject<_ResponseDto>(r);

                if (dto != null && dto.Valid)
                {
                    var labels = JsonConvert.DeserializeObject<List<LangLabelDto>>(dto.Result.ToString());
                    foreach (var l in labels)
                        dic.Add(l.LangKeyCode, l);
                }
            }
            catch { }
            return dic;
        }

        public bool IsLabel(string labelCode, Dictionary<string, LangLabelDto> labels) => labels != null && labelCode != null && labels.ContainsKey(labelCode);

        public string GetLabel(string labelCode, Dictionary<string, LangLabelDto> labels, bool isDev) => 
            IsLabel(labelCode, labels) ? 
            labels[labelCode].Label : 
            (isDev?"[":"") + labelCode + (isDev ? "]" : "");

    }
}
