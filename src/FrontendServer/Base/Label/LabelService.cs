using Common.DTO.Base;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Newtonsoft.Json;
using GC = FrontendServer.GlobalConstants;

/// <summary>
/// Scoped label service for the client
/// Created: June 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>
namespace FrontendServer.Base.Config
{
    public class LabelService : BaseService
    {
        private readonly ConfigService _configService;
        private readonly LabelCacheService _labelCacheService;

        public LabelService(ProtectedSessionStorage session,
            IHttpClientFactory httpClientFactory,
            ConfigService configService,
            LabelCacheService labelCacheService)
        {
            _session = session;
            _httpClientFactory = httpClientFactory;
            _configService = configService;
            _labelCacheService = labelCacheService;
        }

        private Dictionary<string, LangLabelDto> labels;

        public event Action? OnLabelsChanged;
        private bool IsInitialising = false;

        public Dictionary<string, LangLabelDto>? Labels => labels;
        public void SetLabels(Dictionary<string, LangLabelDto> labels)
        {
            this.labels = labels;
            OnLabelsChanged?.Invoke();
        }

        public bool IsInitialized => labels != null;

        public bool IsLabel(string labelCode) => labels != null && labelCode != null && labels.ContainsKey(labelCode);
        public bool IsTooltip(string labelCode) => IsLabel(labelCode) ? !string.IsNullOrEmpty(labels[labelCode].Tooltip) : false;

        public string GetLabel(string labelCode) => IsLabel(labelCode) ? labels[labelCode].Label : labelCode;
        public string GetLabelHighlightNoKey(string labelCode) => IsLabel(labelCode) ? labels[labelCode].Label : "[" + labelCode + "]";
        public string? GetTooltip(string labelCode) => IsTooltip(labelCode) ? labels[labelCode].Tooltip : null;

        public async Task<Dictionary<string, LangLabelDto>> Initialise(LoginParameters lps)
        {
            try
            {
                if (!_configService.IsInitialized)
                    await _configService.Initialise();

                var config = _configService.Config;
                var client = await GetClient();

                if (!_labelCacheService.HasLabels(config.Label.LangCode, config.Label.Variant) && !IsInitialising)
                {
                    IsInitialising = true;
                    var url = GC.URL_label_clientlist
                                + config.Label.LangCode
                                + (config.Label.Variant.HasValue ? "/" + config.Label.Variant : "");

                    var rl = await client.GetAsync(url);
                    var rls = await rl.Content.ReadAsStringAsync();
                    var rdto = JsonConvert.DeserializeObject<_ResponseDto>(rls);

                    if (rdto.Valid)
                    {
                        var labels = JsonConvert.DeserializeObject<List<LangLabelDto>>(rdto.Result.ToString());
                        var dic = new Dictionary<string, LangLabelDto>();
                        foreach (var l in labels)
                            dic.Add(l.LangKeyCode, l);
                        _labelCacheService.PutLabels(config.Label.LangCode, config.Label.Variant, dic);
                        lps.LoadedUrl(url);
                    }
                    IsInitialising = false;
                }
                var labelsX = _labelCacheService.GetLabels(config.Label.LangCode, config.Label.Variant);
                SetLabels(labelsX);
                return labelsX;
            }
            catch
            {
                return null;
            }
        }

    }
}
