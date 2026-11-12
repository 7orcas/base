namespace Common.DTO.Base
{
    public class LangCodeAdminDto : _BaseDto
    {
        public int? Variant { get; set; }
        public string? Flag { get; set; }
       
        public Dictionary<long, LangLabelAdminDto> LabelsDic { get; set; }
    }
}
