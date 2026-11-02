
namespace Common.DTO.Base
{
    public class LangLabelAdminDto : _BaseDto
    {
        public long Id { get; set; }
        public long LangCodeId { set; get; }
        public long LangKeyId { set; get; }
        public int? Variant { get; set; }
        public string Key { get; set; }
        public string Code { get; set; }
        public string? Tooltip { get; set; }
    }
}
