using Common.DTO.Base;
using Microsoft.Extensions.Caching.Memory;
using GC = FrontendServer.GlobalConstants;

/// <summary>
/// Singleton service to manage labels for all languages
/// Created: June 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace FrontendServer.Base.Label
{
    public class LabelCacheService
    {
        private readonly IMemoryCache _cache;

        public LabelCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        static public string LabelKey(string key, int? variant) => GC.LabelCacheKey + "-" + key + (variant.HasValue? "-" + variant : "");
        

        public bool HasLabels(string langCode, int? variant)
        {
            if (_cache.TryGetValue(LabelKey(langCode, variant), out Dictionary<string, LangLabelDto> labelsX))
                return true;
            return false;
        }

        public void PutLabels(string langCode, int? variant, Dictionary<string, LangLabelDto> labels) => _cache.Set(LabelKey(langCode, variant), labels);
        

        public Dictionary<string, LangLabelDto> GetLabels(string langCode, int? variant)
        {
            Dictionary<string, LangLabelDto>? labels = null;

            if (_cache.TryGetValue(LabelKey(langCode, variant), out Dictionary<string, LangLabelDto> labelsX))
                labels = labelsX;

            return labels != null ? labels : new Dictionary<string, LangLabelDto>();
        }

    }
}
