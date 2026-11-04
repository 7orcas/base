namespace Common.DTO.Base
{
    public class LangCodeAdminDto : _BaseDto
    {
        public string? Flag { get; set; }
        public List<LangLabelAdminDto> Labels;
        public Dictionary<long, LangLabelAdminDto> LabelsDic;
    }
}
