namespace Common.DTO.Base
{

    public class ConfigDto : _BaseDto
    {
        public long UniqueUserId { get; set; }
        public long UniqueSessionId { get; set; }
        public OrgConfigDto Org { get; set; }
        public LanguageConfigDto[] Languages { get; set; }
        public LabelConfigDto Label { get; set; }
        public UserConfigDto User { get; set; }
        public bool DebugMode { get; set; } = false;
        public string UrlLogin { get; set; }

        public static ConfigDto CreateEmpty()
        {
            return new ConfigDto
            {
                Org = new OrgConfigDto(),
                Label = new LabelConfigDto(),
                User = new UserConfigDto(),
                Languages = Array.Empty<LanguageConfigDto>(),
            };
        }
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
        public bool IsAdmin { get; set; } = false;
        public bool HighlightNoKey { get; set; } = false;
    }

    public class LanguageConfigDto
    {
        public string LangCode { get; set; }
        public bool IsUpdateable { get; set; } = false;
    }

    public class UserConfigDto
    {
        public string Username { get; set; }
        public bool IsService { get; set; } = false;
        public bool IsAdmin { get; set; } = false;
    }

}
