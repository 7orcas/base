using System;

namespace Backend.Base.Audit.Ent
{
    public class AuditEnt
    {
        public long Id {  get; set; }
        public int OrgNr { get; set; }
        public int Source {  get; set; }
        public int EntityTypeNr {  get; set; }
        public long? EntityId { get; set; }
        public long UserAccId { get; set; }
        public long? MasqueradeId { get; set; }
        public DateTimeOffset Created {  get; set; }
	    public string? Crud {  get; set; }
        public string? Details { get; set; }
        
    }
}
