using System;
using System.ComponentModel.DataAnnotations.Schema;

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
        
        [NotMapped]
        public string? UserName { get; set; }
        [NotMapped]
        public string? Masquerade { get; set; }
        [NotMapped]
        public string EntityType { get; set; }
        [NotMapped]
        public string CrudDescr { get; set; }
        [NotMapped]
        public string SourceDescr { get; set; }

    }
}
