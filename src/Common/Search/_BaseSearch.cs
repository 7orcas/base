
namespace Common.Search
{
    public class _BaseSearch
    {
        public bool IncludeActive { get; set; } = true;
        public bool IncludeInActive { get; set; } = false;
        public int MaxRecordsReturned { get; set; } = 500;
    }
}
