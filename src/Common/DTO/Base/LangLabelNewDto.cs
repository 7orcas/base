namespace Common.DTO.Base
{
    public class LangLabelNewDto : _BaseDto
    {
        public long LangCodeId { get; set; }
        public long LangKeyId { get; set; }
        public int? Variant { get; set; }
    }
}
