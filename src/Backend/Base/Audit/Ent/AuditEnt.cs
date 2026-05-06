using System;

namespace Backend.Base.Audit.Ent
{
    public class AuditEnt
    {
        public long Id {  get; set; }
        public long orgNr { get; set; }
        public int Source {  get; set; }
        public int EntityTypeId {  get; set; }
        public long? EntityId { get; set; }
        public long UserId { get; set; }
        public DateTime Created {  get; set; }
	    public string? Crud {  get; set; }
        public string? Details { get; set; }
        
    }
}
