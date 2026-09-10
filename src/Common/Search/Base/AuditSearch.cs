
namespace Common.Search.Base
{
    public class AuditSearch : _BaseSearch
    {
        public int? OrgNr { get; set; }
        public int? Source { get; set; }
        public int? EntityTypeNr { get; set; }
        public long? EntityId { get; set; }
        public string? Username { get; set; }
        public string? CRUD { get; set; }

        public override bool ShowActive => false;
    }
}
