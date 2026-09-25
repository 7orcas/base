using static Common.GlobalConstants;
using GC = Common.GlobalConstants;

namespace Common.Search
{
    public abstract class _BaseSearch
    {
        public bool IncludeActive { get; set; } = true;
        public bool IncludeInActive { get; set; } = false;
        public virtual bool ShowActive { get; set; } = true;
        public int MaxRecordsReturned { get; set; } = 500;
        public TextSearchCompare TextFieldSearchType { get; set; } = TextSearchCompare.Start;
        public TextSearchDelimiter TextFieldDelimiterType { get; set; } = TextSearchDelimiter.None;
        public bool IsTextFieldSearchUnaccent { get; set; } = false;
        public bool RememberSearch { get; set; } = false;

        public virtual bool IsValid() => true;
    }
}
