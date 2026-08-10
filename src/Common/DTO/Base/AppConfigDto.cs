namespace Common.DTO.Base
{
    public class AppConfigDto : _BaseDto<AppConfigDto>
    {
        public OrgConfigDto Org { get; set; }
        public long UniqueUserId { get; set; }
        public long UniqueSessionId { get; set; }
        public LanguageConfigDto[] Languages { get; set; }
        public LabelConfigDto Label { get; set; }
        public UserConfigDto User { get; set; }
        public bool DebugMode { get; set; } = false;
        public string UrlLogin { get; set; }
    }

    public class OrgConfigDto
    {
        public int Nr { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
    }


    public class LabelConfigDto
    {
        //Current language code
        public string LangCode { get; set; }
        public int? Variant { get; set; }
        public bool ShowTooltip { get; set; } = false;
        public bool IsAdminLanguage { get; set; } = false;
        public bool HighlightNoKey { get; set; } = false;
    }

    public class LanguageConfigDto
    {
        public string LangCode { get; set; }
        public bool IsUpdateable { get; set; } = false;
    }

    public class UserConfigDto
    {
        public bool IsService { get; set; } = false;
        public bool IsAdminUser { get; set; } = false;
        public bool IsAdminLang { get; set; } = false;
    }

}
