
namespace Common.DTO.Base
{
    public class LangLabelAdminDto : _BaseDto
    {
        public long Id { get; set; }
        public long LangCodeId { set; get; }
        public long LangKeyId { set; get; }
        public int? Variant { get; set; }
        public string LangKey { get; set; }
        public string Label { get; set; }
    }
}
