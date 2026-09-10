using GC = Common.GlobalConstants;

namespace Common.Search
{
    public abstract class _BaseSearch
    {
        public bool IncludeActive { get; set; } = true;
        public bool IncludeInActive { get; set; } = false;
        public virtual bool ShowActive { get; set; } = true;
        public int MaxRecordsReturned { get; set; } = 500;
        public int TextFieldSearchType { get; set; } = GC.TextSearchStart;
        public bool IsTextFieldSearchUnaccent { get; set; } = false;

        public virtual bool IsValid() => true;
    }
}
