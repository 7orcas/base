
namespace Common.Search.Base
{
    public class AuditSearch : _BaseSearch
    {
        public int? OrgNr { get; set; }
        public int? Source { get; set; }
        public string? EntityType { get; set; }
        public long? EntityId { get; set; }
        public string? Username { get; set; }
        public string? CRUD { get; set; }
        public DateTime? FromDate { get; set; }
        public TimeSpan? FromTime { get; set; }
        public DateTime? ToDate { get; set; }

        public override bool ShowActive => false;
    }
}
