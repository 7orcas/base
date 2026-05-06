using Common.DTO.Base;
using Microsoft.AspNetCore.Components;

/// <summary>
/// Object to store initial login parameters
/// Used in debug
/// Created: July 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace FrontendServer.Base.Util
{
    public class LoginParameters
    {
        public string? tokenkey { get; set; }
        public string? token { get; set; }
        public string? org { get; set; }
        public string? lang { get; set; }
        public int? langVariant { get; set; }
        public string? userId { get; set; }
        public string? sessionId { get; set; }
        public MarkupString loaded { get; set; }

        public void Set(AppConfigDto config)
        {
            if (config == null) { throw new ArgumentNullException("config"); }
            org = config.OrgDescription;
            lang = config.Label.LangCode;
            langVariant = config.Label.Variant;
            userId = config.UniqueUserId.ToString();
            sessionId = config.UniqueSessionId.ToString();
        }

        public void LoadedUrl(string url)
        {
            loaded = AppendMarkupString(loaded, url);
        }

        private MarkupString AppendMarkupString(MarkupString m, string s)
        {
            if (string.IsNullOrEmpty(m.ToString()))
                return new MarkupString(s);

            return new MarkupString(m.ToString() + "<br>" + s);
        }

    }
}
