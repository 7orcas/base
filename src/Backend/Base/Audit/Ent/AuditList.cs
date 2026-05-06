namespace Backend.Base.Audit.Ent
{
    public class AuditList : AuditEnt
    {
        public string EntityType { get; set; }
        public string? User { get; set; }
        public string CrudDescr { get; set; }
        public string SourceDescr { get; set; }
    }
}
