namespace Common.DTO.Base
{
    public class AppConfigDto : _BaseDto<AppConfigDto>
    {
        public string OrgDescription { get; set; }
        public long UniqueUserId { get; set; }
        public long UniqueSessionId { get; set; }
        public LanguageConfigDto[] Languages { get; set; }
        public LabelConfigDto Label { get; set; }
        public UserConfigDto User { get; set; }
        public bool DebugMode { get; set; } = false;
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
        public bool IsSystemAdmin { get; set; } = false;
        public bool IsCurrentLanguageAdmin { get; set; } = false;
        public bool IsActiveLanguageAdmin { get; set; } = false;
    }

}
