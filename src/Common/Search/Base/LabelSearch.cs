
namespace Common.Search.Base
{
    public class LabelSearch : _BaseSearch
    {
        public long? LangCodeId { get; set; } //for label loads
        public string? LangCode { get; set; }
        public string? LangKey { get; set; }
        public string? LangLabel { get; set; }
        public int? Variant { get; set; }
    }

}
